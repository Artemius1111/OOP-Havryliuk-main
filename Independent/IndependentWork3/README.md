# Звіт до Самостійної роботи №3
**Тема:** Аналіз інкапсуляції в open-source проєктах. Практики валідації полів

## 1. Обраний проєкт
* **Назва:** .NET (`dotnet/runtime`).
* **Репозиторій:** [github.com/dotnet/runtime](https://github.com/dotnet/runtime).
* **Проаналізовані класи:** [`UriBuilder`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.Uri/src/System/UriBuilder.cs), [`HttpRequestMessage`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Net.Http/src/System/Net/Http/HttpRequestMessage.cs) та [`HttpResponseMessage`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Net.Http/src/System/Net/Http/HttpResponseMessage.cs).

## 2. Аналіз інкапсуляції та валідації
* **`UriBuilder`:** Зберігає частини URI у приватних полях, а властивості забезпечують контрольований доступ. Сеттер `Path` нормалізує роздільники та екранує рядок; `Query` і `Fragment` додають необхідні початкові символи. Сеттер `Scheme` перевіряє назву схеми, а `Port` відхиляє значення поза діапазоном від `-1` до `65535`.
* **`HttpRequestMessage`:** Інкапсулює метод запиту, URI, версію, заголовки та вміст. Властивості `Method` і `Version` перевіряють значення на `null`, `VersionPolicy` — допустимість значення переліку. Зміна властивостей також перевіряє, чи повідомлення не було утилізоване.
* **`HttpResponseMessage`:** Інкапсулює код відповіді, версію, заголовки, вміст і фразу стану. `StatusCode` перевіряє діапазон від `0` до `999`, `ReasonPhrase` відхиляє рядки з неприпустимими символами нового рядка, а `Version` — значення `null`. `IsSuccessStatusCode` доступна лише для читання й визначає, чи належить код до діапазону 200–299.
* Перевірки виконуються у властивостях і конструкторах; для некоректних значень використовуються винятки `ArgumentException`, `ArgumentOutOfRangeException`, `ArgumentNullException` і `FormatException`.
* У розглянутих класах властивості переважно працюють із приватними полями, а не є автоматичними. Індексатори та перевантажені оператори для описаних сценаріїв не використовуються.

## 3. Висновки
* Інкапсуляція приховує внутрішній стан і дає змогу контролювати його через властивості та методи.
* Валідація безпосередньо під час зміни значення допомагає рано виявити некоректні дані. Конкретні перевірки залежать від призначення властивості.
* Посилання на вихідний код: [репозиторій dotnet/runtime](https://github.com/dotnet/runtime) та файли [`UriBuilder.cs`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.Uri/src/System/UriBuilder.cs), [`HttpRequestMessage.cs`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Net.Http/src/System/Net/Http/HttpRequestMessage.cs), [`HttpResponseMessage.cs`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Net.Http/src/System/Net/Http/HttpResponseMessage.cs).

## 4. Відповіді на контрольні запитання
1. **За якими ознаками можна зробити висновок, що в класі дотримано інкапсуляції?**
   Дані зберігаються у приватних полях, а публічні властивості та методи контролюють доступ і зміни.
2. **Які підходи до валідації властивостей зустрічаються в коді?**
   Перевірка `null`, діапазонів чисел, значень переліків, формату рядків і стану об'єкта з використанням винятків.
3. **Чому важливо наводити посилання на конкретні класи та фрагменти коду?**
   Вони дають змогу перевірити висновки та побачити їх у контексті першоджерела.
4. **Які висновки щодо якості проєктування дає порівняння класів?**
   Класи приховують стан і перевіряють важливі значення; правила перевірки відрізняються залежно від контракту кожної властивості.
