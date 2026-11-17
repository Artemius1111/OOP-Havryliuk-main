# Лабораторна робота №14: Аналіз SRP/OCP у відкритих репозиторіях.

**Тема:** Аналіз SRP/OCP у відкритих репозиторіях.

**Мета:** Дослідити, як принципи SRP (Single Responsibility Principle) та OCP (Open/Closed Principle) застосовуються в реальних проєктах, і підготувати короткий практичний аналіз прикладів та пропозицію рефакторингу.


## Хід роботи

Для аналізу обрано два open-source репозиторії на C# з різних доменів. У кожному знайдено приклади вдалого застосування та порушення SRP і приклади розширення за OCP, проаналізовано їх вплив на підтримку та розширення і запропоновано рефакторинг для найгіршого прикладу.

## 1. Обрані репозиторії

| № | Репозиторій | Домен | Зафіксований коміт |
|---|---|---|---|
| 1 | [dotnet-architecture/eShopOnWeb](https://github.com/dotnet-architecture/eShopOnWeb) | Інтернет-магазин (ASP.NET Core, Clean Architecture) | [`4da8212`](https://github.com/dotnet-architecture/eShopOnWeb/tree/4da8212117e87d808d4bbc7da6286fd2147ce606) |
| 2 | [jellyfin/jellyfin](https://github.com/jellyfin/jellyfin) | Медіасервер (бібліотека фільмів, музики, стрімінг) | [`07bcec2`](https://github.com/jellyfin/jellyfin/tree/07bcec2e1f4e835bc3a667c856dd444ec46b4611) |

Усі посилання нижче прив’язані до конкретного коміту, тому номери рядків не «з’їдуть», якщо в репозиторії з’являться нові зміни.

## 2. Приклади вдалого застосування SRP

### 2.1. eShopOnWeb — `UriComposer` і `OrderService`

**Де в коді:**
- [`src/ApplicationCore/Services/UriComposer.cs`, рядки 5–15](https://github.com/dotnet-architecture/eShopOnWeb/blob/4da8212117e87d808d4bbc7da6286fd2147ce606/src/ApplicationCore/Services/UriComposer.cs#L5-L15)
- [`src/ApplicationCore/Services/OrderService.cs`, рядки 12–53](https://github.com/dotnet-architecture/eShopOnWeb/blob/4da8212117e87d808d4bbc7da6286fd2147ce606/src/ApplicationCore/Services/OrderService.cs#L12-L53)

```csharp
public class UriComposer : IUriComposer
{
    private readonly CatalogSettings _catalogSettings;
    public UriComposer(CatalogSettings catalogSettings) => _catalogSettings = catalogSettings;

    public string ComposePicUri(string uriTemplate)
    {
        return uriTemplate.Replace("http://catalogbaseurltobereplaced", _catalogSettings.CatalogBaseUrl);
    }
}
```

`UriComposer` робить рівно одну річ — підставляє базову адресу каталогу в шлях до картинки. `OrderService` має один публічний метод `CreateOrderAsync`: перетворює кошик на замовлення. Він не знає, як дані зберігаються (працює через `IRepository<T>`), не формує URL сам (делегує `IUriComposer`), не відправляє листів і не працює з HTTP.

**Переваги / ризики:** кожен клас має одну причину для зміни. Якщо зміниться CDN для картинок — правимо тільки `UriComposer`; якщо зміниться база даних — тільки реалізацію репозиторію. Класи легко тестувати з моками. Ризик мінімальний: трохи більше файлів та інтерфейсів.

**При додаванні нової функції:** наприклад, «надіслати лист після створення замовлення» — це нова відповідальність, тому правильно додати окремий сервіс (`IOrderNotificationService`), а не дописувати відправку пошти в `OrderService`. Поточна структура прямо підштовхує до такого рішення.

### 2.2. Jellyfin — `CleanActivityLogTask` + `ScheduledTaskWorker`

**Де в коді:**
- [`Emby.Server.Implementations/ScheduledTasks/Tasks/CleanActivityLogTask.cs`, рядки 15–76](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/Emby.Server.Implementations/ScheduledTasks/Tasks/CleanActivityLogTask.cs#L15-L76)
- [`Emby.Server.Implementations/ScheduledTasks/TaskManager.cs`, рядки 176–181](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/Emby.Server.Implementations/ScheduledTasks/TaskManager.cs#L176-L181)

```csharp
public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
{
    var retentionDays = _serverConfigurationManager.Configuration.ActivityLogRetentionDays;
    if (!retentionDays.HasValue || retentionDays < 0)
        throw new InvalidOperationException(...);

    var startDate = DateTime.UtcNow.AddDays(-retentionDays.Value);
    return _activityManager.CleanAsync(startDate);
}
```

Задача відповідає лише за **що** зробити — видалити старі записи журналу активності. **Коли** і **як** її запускати (тригери, збереження стану, історія запусків, скасування) — відповідальність окремого класу `ScheduledTaskWorker` (678 рядків), у який `TaskManager.AddTasks` обгортає кожну задачу. Навіть саме видалення делеговано `IActivityManager`.

**Переваги / ризики:** логіка планувальника написана один раз і перевикористовується для ~20 задач. Зміна правил планування не зачіпає задач, а зміна логіки очищення не зачіпає планувальника. Ризик — щоб зрозуміти повну поведінку, треба відкрити 2–3 класи.

**При додаванні нової функції:** нова фонова задача — це один невеликий клас на ~60 рядків, без жодного коду про таймери чи потоки.

## 3. Приклади порушення SRP

### 3.1. eShopOnWeb — `CheckoutModel` (сторінка оформлення замовлення)

**Де в коді:** [`src/Web/Pages/Basket/Checkout.cshtml.cs`, рядки 15–97](https://github.com/dotnet-architecture/eShopOnWeb/blob/4da8212117e87d808d4bbc7da6286fd2147ce606/src/Web/Pages/Basket/Checkout.cshtml.cs#L15-L97), особливо [рядки 55–58](https://github.com/dotnet-architecture/eShopOnWeb/blob/4da8212117e87d808d4bbc7da6286fd2147ce606/src/Web/Pages/Basket/Checkout.cshtml.cs#L55-L58) і [84–96](https://github.com/dotnet-architecture/eShopOnWeb/blob/4da8212117e87d808d4bbc7da6286fd2147ce606/src/Web/Pages/Basket/Checkout.cshtml.cs#L84-L96)

```csharp
await _basketService.SetQuantities(BasketModel.Id, updateModel);
await _orderService.CreateOrderAsync(BasketModel.Id,
    new Address("123 Main St.", "Kent", "OH", "United States", "44240"));
await _basketService.DeleteBasketAsync(BasketModel.Id);
```

Один клас сторінки поєднує кілька відповідальностей:
1. обробку HTTP-запиту та валідацію форми;
2. **сценарій оформлення** (оновити кількості → створити замовлення → видалити кошик) — це бізнес-логіка, якій не місце в UI;
3. **визначення адреси доставки** — вона захардкоджена прямо в UI;
4. **керування cookie анонімного покупця** (`GetOrSetBasketCookieAndUserName`). Цей самий метод скопійовано й у [`Pages/Basket/Index.cshtml.cs`](https://github.com/dotnet-architecture/eShopOnWeb/blob/4da8212117e87d808d4bbc7da6286fd2147ce606/src/Web/Pages/Basket/Index.cshtml.cs) — дублювання є наслідком порушення SRP.

**Ризики:** три кроки оформлення не атомарні та не тестуються без веб-контексту; при зміні політики cookie треба правити кілька сторінок; захардкоджена адреса — прихована бізнес-помилка.

**При додаванні нової функції:** наприклад, «застосувати промокод» чи «вибір адреси доставки» — доведеться знову редагувати клас сторінки, і він ростиме. Також API-клієнт (`PublicApi`) не зможе перевикористати цей сценарій оформлення, бо він «заперт» у Razor-сторінці.

### 3.2. Jellyfin — `LibraryManager` (God Object)

**Де в коді:** [`Emby.Server.Implementations/Library/LibraryManager.cs`](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/Emby.Server.Implementations/Library/LibraryManager.cs) — **4234 рядки**, [26 залежностей у полях (рядки 68–93)](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/Emby.Server.Implementations/Library/LibraryManager.cs#L68-L93).

Один клас відповідає за зовсім різні речі (вибірково):

| Відповідальність | Рядки |
|---|---|
| Видалення елементів і файлів з диска | [347–684](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/Emby.Server.Implementations/Library/LibraryManager.cs#L347-L684) |
| Розпізнавання файлів (resolve) | [766–1100](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/Emby.Server.Implementations/Library/LibraryManager.cs#L766-L1100) |
| Люди, студії, жанри, виконавці | [1221–1400](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/Emby.Server.Implementations/Library/LibraryManager.cs#L1221-L1400) |
| Сканування бібліотеки | [1403–1605](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/Emby.Server.Implementations/Library/LibraryManager.cs#L1403-L1605) |
| Сортування | [2341–2438](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/Emby.Server.Implementations/Library/LibraryManager.cs#L2341-L2438) |
| Парсинг номерів серій з шляху | [3305–3440](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/Emby.Server.Implementations/Library/LibraryManager.cs#L3305-L3440) |
| Пошук «екстра»-файлів (трейлери тощо) | [3442–3635](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/Emby.Server.Implementations/Library/LibraryManager.cs#L3442-L3635) |
| Керування віртуальними папками (медіатеками) | [3807–4166](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/Emby.Server.Implementations/Library/LibraryManager.cs#L3807-L4166) |

**Ризики:** у класу десятки причин для зміни, тому конфлікти злиття між розробниками неминучі. Unit-тестування потребує 26 моків у конструкторі. Будь-яка зміна (наприклад, в парсингу серій) вимагає перекомпіляції й повторного тестування класу, від якого залежить майже весь сервер.

**При додаванні нової функції:** наприклад, «перейменування медіатеки» — розробник знову допише метод у цей самий клас, бо вся пов’язана логіка вже там. Клас продовжить рости. Варто зазначити, що команда Jellyfin вже почала виносити частини в окремі сервіси (`INextUpService`, `IItemCountService`, `ILinkedChildrenService` у полях 80–82), тобто сама визнає проблему.

## 4. Приклади розширення за OCP

### 4.1. eShopOnWeb — Декоратор кешування + Specification

**Де в коді:**
- [`src/Web/Services/CachedCatalogViewModelService.cs`, рядки 10–49](https://github.com/dotnet-architecture/eShopOnWeb/blob/4da8212117e87d808d4bbc7da6286fd2147ce606/src/Web/Services/CachedCatalogViewModelService.cs#L10-L49)
- реєстрація: [`src/Web/Configuration/ConfigureWebServices.cs`, рядки 14 і 17](https://github.com/dotnet-architecture/eShopOnWeb/blob/4da8212117e87d808d4bbc7da6286fd2147ce606/src/Web/Configuration/ConfigureWebServices.cs#L14-L17)
- специфікації: [`src/ApplicationCore/Specifications/CatalogFilterSpecification.cs`](https://github.com/dotnet-architecture/eShopOnWeb/blob/4da8212117e87d808d4bbc7da6286fd2147ce606/src/ApplicationCore/Specifications/CatalogFilterSpecification.cs#L6-L13), використання в [`CatalogViewModelService.cs`, рядки 44–50](https://github.com/dotnet-architecture/eShopOnWeb/blob/4da8212117e87d808d4bbc7da6286fd2147ce606/src/Web/Services/CatalogViewModelService.cs#L44-L50)

```csharp
services.AddScoped<CatalogViewModelService>();
services.AddScoped<ICatalogViewModelService, CachedCatalogViewModelService>();
```

Кешування додано **без жодної зміни** в `CatalogViewModelService`: новий клас реалізує той самий інтерфейс `ICatalogViewModelService` і обгортає оригінал (патерн Декоратор). Перемикання — один рядок у DI. Аналогічно, `IRepository<T>` закритий для змін: нові запити до БД додаються новими класами-специфікаціями (`CatalogFilterSpecification`, `BasketWithItemsSpecification`…), а не новими методами репозиторію.

**Переваги / ризики:** перевірений код не чіпаємо; кеш можна вимкнути заміною реєстрації. Ризик: декоратор дублює всі методи інтерфейсу — при додаванні методу в `ICatalogViewModelService` доведеться оновлювати обидва класи.

**При додаванні нової функції:** наприклад, логування часу запитів — ще один декоратор поверх кешуючого; новий фільтр каталогу — нова специфікація.

### 4.2. Jellyfin — плагінні точки розширення (`IScheduledTask`, `IBaseItemComparer`)

**Де в коді:**
- інтерфейс: [`MediaBrowser.Model/Tasks/IScheduledTask.cs`, рядки 11–49](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/MediaBrowser.Model/Tasks/IScheduledTask.cs#L11-L49)
- автоматичне підключення всіх реалізацій: [`Emby.Server.Implementations/ApplicationHost.cs`, рядок 424](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/Emby.Server.Implementations/ApplicationHost.cs#L424) та [рядки 730–743](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/Emby.Server.Implementations/ApplicationHost.cs#L730-L743)
- приклад стратегії сортування: [`Emby.Server.Implementations/Sorting/RuntimeComparer.cs`](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/Emby.Server.Implementations/Sorting/RuntimeComparer.cs#L12-L33) і пошук стратегії в [`LibraryManager.GetComparer`, рядки 2416–2437](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/Emby.Server.Implementations/Library/LibraryManager.cs#L2416-L2437)

```csharp
Resolve<ITaskManager>().AddTasks(GetExports<IScheduledTask>(false));
...
Resolve<ILibraryManager>().AddParts(
    GetExports<IResolverIgnoreRule>(),
    GetExports<IItemResolver>(),
    GetExports<IIntroProvider>(),
    GetExports<IBaseItemComparer>(),
    GetExports<ILibraryPostScanTask>());
```

```csharp
var comparer = Comparers.FirstOrDefault(c => name == c.Type);   // без switch/if по типах
```

`GetExports<T>()` знаходить усі класи, що реалізують інтерфейс, — і в ядрі, і в **сторонніх плагінах**. Тому нова фонова задача, новий провайдер метаданих чи нове правило сортування додаються новим класом; `TaskManager` і `LibraryManager` не змінюються. Сортування — класичний патерн Стратегія: 26 класів у папці `Sorting/`, вибір за властивістю `Type`, без `switch`.

**Переваги / ризики:** сторонні розробники розширюють сервер, не маючи доступу до його коду (так працюють плагіни Jellyfin). Ризик: OCP тут не «чистий» — для нового сортування все одно треба додати значення в enum [`Jellyfin.Data/Enums/ItemSortBy.cs`](https://github.com/jellyfin/jellyfin/blob/07bcec2e1f4e835bc3a667c856dd444ec46b4611/Jellyfin.Data/Enums/ItemSortBy.cs), тобто плагін не може додати власний ключ сортування. Також помилки реєстрації видно лише під час виконання.

**При додаванні нової функції:** наприклад, «сортування за бітрейтом» — новий клас `BitrateComparer : IBaseItemComparer` + одне значення в enum; «щотижневе резервне копіювання» — новий клас `BackupTask : IScheduledTask`, і він автоматично з’явиться в адмін-панелі.

## 5. Пропозиція рефакторингу для найгіршого прикладу

Найгірший приклад — `LibraryManager` у Jellyfin (4234 рядки, 26 залежностей). Переписати його повністю за раз нереально, тому пропоную поступове виділення відповідальностей, починаючи з **керування віртуальними папками** (рядки 3807–4166). Ця частина найменше пов’язана з рештою класу: вона працює з файловою системою та конфігурацією, а не з елементами бібліотеки.

**Крок 1.** Створити окремий інтерфейс і клас:

```csharp
public interface IVirtualFolderService
{
    List<VirtualFolderInfo> GetVirtualFolders(bool includeRefreshState = false);
    Task AddVirtualFolder(string name, CollectionTypeOptions? collectionType,
                          LibraryOptions options, bool refreshLibrary);
    Task RemoveVirtualFolder(string name, bool refreshLibrary);
    void AddMediaPath(string virtualFolderName, MediaPathInfo mediaPath);
    void UpdateMediaPath(string virtualFolderName, MediaPathInfo mediaPath);
    void RemoveMediaPath(string virtualFolderName, string mediaPath);
}

public class VirtualFolderService : IVirtualFolderService
{
    private readonly IFileSystem _fileSystem;
    private readonly IServerConfigurationManager _configurationManager;
    private readonly ILibraryMonitor _libraryMonitor;
    private readonly ITaskManager _taskManager;   // для повторного сканування

    // сюди переносяться AddVirtualFolder, AddMediaPathInternal,
    // SyncLibraryOptionsToLocations, RemoveContentTypeOverrides, CreateShortcut ...
}
```

**Крок 2.** Щоб не зламати контролери та плагіни, які викликають `ILibraryManager.AddVirtualFolder(...)`, у `LibraryManager` тимчасово залишити методи-делегати з атрибутом `[Obsolete]`, що просто викликають `IVirtualFolderService`. Через 1–2 релізи — видалити.

**Крок 3.** Перевести `LibraryStructureController` (API) на прямий виклик `IVirtualFolderService`.

**Обґрунтування:**
- `LibraryManager` зменшиться приблизно на 360 рядків і позбудеться частини залежностей;
- нову функцію «перейменувати медіатеку» можна буде додати в маленький клас, не чіпаючи логіку сканування й видалення;
- `VirtualFolderService` тестується з 4 моками замість 26;
- підхід узгоджується з тим, що команда вже робить (`INextUpService`, `IItemCountService`), тож це продовження наявного напряму, а не рефакторинг «під патерн».

Наступні кандидати на виділення за тим самим принципом — `EpisodeNumberParser` (рядки 3305–3440) та `ExtrasFinder` (3442–3635).

## Висновок

1. В обох репозиторіях SRP найкраще дотримано там, де класи маленькі й мають одну причину для зміни: `UriComposer`, `OrderService`, окремі `IScheduledTask`.
2. Великий обсяг коду ще не означає порушення SRP — `ScheduledTaskWorker` має 678 рядків, але всі вони про одне: запуск задачі за розкладом.
3. Справжня ознака порушення — різні **причини для змін** в одному класі: у `CheckoutModel` це UI, бізнес-сценарій і cookie, у `LibraryManager` — понад вісім різних областей.
4. Порушення SRP спричиняє дублювання (метод cookie у двох сторінках eShopOnWeb) і величезну кількість залежностей (26 у `LibraryManager`).
5. OCP в обох проєктах реалізовано через абстракції + DI: Декоратор і Specification в eShopOnWeb, Стратегія та плагінне підключення через `GetExports<T>()` у Jellyfin.
6. Найкращий доказ дотримання OCP — точки розширення, де нова поведінка додається новим класом без змін існуючого коду; самі по собі інтерфейси цього не гарантують.
7. OCP рідко буває «повним»: у Jellyfin для нового сортування все одно потрібно змінити enum `ItemSortBy`.
8. Навчальний проєкт (eShopOnWeb) чистіший, але навіть у ньому є порушення; великий реальний проєкт (Jellyfin) має чудові точки розширення поруч із God Object, що ілюструє поступове накопичення технічного боргу.
9. Рефакторинг великих класів варто робити поступово, виділяючи по одній відповідальності та зберігаючи сумісність через делегування.

## Відповіді на контрольні питання

**1. Як відрізнити окрему відповідальність від просто великого обсягу коду?**
Відповідальність — це причина для зміни (або «актор», який може цю зміну попросити). Якщо всі методи класу змінюються з однієї причини, клас може бути великим і все одно дотримуватися SRP (`ScheduledTaskWorker`). Ознаки кількох відповідальностей: групи полів, які використовуються лише частиною методів; залежності з різних шарів (HTTP + БД + файли); назва з «Manager»/«Helper», яку важко описати одним реченням.

**2. Які ознаки найкраще підтверджують дотримання OCP?**
Нова поведінка додається новим класом, що реалізує інтерфейс, а існуючий код не змінюється; відсутність `switch`/`if` за типами; реєстрація реалізацій через DI або автоматичний пошук (`GetExports<T>()`); наявність декораторів/стратегій/плагінів.

**3. Чому важливо дивитися не лише на інтерфейси, а й на місця розширення?**
Інтерфейс може існувати, але використовуватися з однією реалізацією через `new` або з `switch` по типах — тоді систему все одно доведеться змінювати. Справжній OCP видно у точці, де система збирає реалізації (DI-контейнер, `AddParts`, `GetExports`). Приклад: `IBaseItemComparer` зроблено добре, але enum `ItemSortBy` обмежує розширення.

**4. Які ризики рефакторингу «під патерн» без реальної потреби?**
Зайва складність: багато інтерфейсів з однією реалізацією, важче читати й налагоджувати код; витрата часу без користі; абстракції, що вгадують майбутнє, часто виявляються невдалими й заважають реальним змінам; ризик внести помилки в робочий код. Рефакторинг має бути відповіддю на реальну проблему (часті зміни, дублювання, неможливість тестування).
