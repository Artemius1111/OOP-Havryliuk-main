using Xunit;
using lab7v4;

namespace lab7v4.Tests;

public class UnitTest1
{
    private readonly InMemoryRepository<Category> _repository = new();

    [Fact]
    public void Add_ShouldAddNewItemSuccessfully()
    {
        var category = new Category { Id = 1, Name = "Електроніка", Description = "Техніка" };
        _repository.Add(category);

        var result = _repository.GetById(1);

        Assert.NotNull(result);
        Assert.Equal("Електроніка", result.Name);
    }

    [Fact]
    public void Add_DuplicateId_ShouldThrowInvalidOperationException()
    {
        var category1 = new Category { Id = 1, Name = "Категорія 1", Description = "Опис 1" };
        var category2 = new Category { Id = 1, Name = "Категорія 2", Description = "Опис 2" };

        _repository.Add(category1);

        Assert.Throws<InvalidOperationException>(() => _repository.Add(category2));
    }

    [Fact]
    public void GetById_ExistingId_ShouldReturnCorrectEntity()
    {
        var category = new Category { Id = 10, Name = "Книги", Description = "Друкована продукція" };
        _repository.Add(category);

        var result = _repository.GetById(10);

        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
    }

    [Fact]
    public void GetById_NonExistingId_ShouldReturnNull()
    {
        Assert.Null(_repository.GetById(999));
    }

    [Fact]
    public void GetAll_ShouldReturnAllItems()
    {
        _repository.Add(new Category { Id = 1, Name = "C1", Description = "D1" });
        _repository.Add(new Category { Id = 2, Name = "C2", Description = "D2" });

        Assert.Equal(2, _repository.GetAll().Count());
    }

    [Fact]
    public void Update_ExistingItem_ShouldUpdateData()
    {
        _repository.Add(new Category { Id = 1, Name = "Стара назва", Description = "Старий опис" });
        _repository.Update(new Category { Id = 1, Name = "Нова назва", Description = "Новий опис" });

        var result = _repository.GetById(1);

        Assert.NotNull(result);
        Assert.Equal("Нова назва", result.Name);
    }

    [Fact]
    public void Update_NonExistingItem_ShouldThrowKeyNotFoundException()
    {
        Assert.Throws<KeyNotFoundException>(() => 
            _repository.Update(new Category { Id = 99, Name = "Test", Description = "Test" }));
    }

    [Fact]
    public void Delete_ExistingItem_ShouldRemoveItem()
    {
        _repository.Add(new Category { Id = 1, Name = "Для видалення", Description = "Test" });
        _repository.Delete(1);

        Assert.Null(_repository.GetById(1));
    }

    [Fact]
    public void Delete_NonExistingItem_ShouldThrowKeyNotFoundException()
    {
        Assert.Throws<KeyNotFoundException>(() => _repository.Delete(555));
    }
}