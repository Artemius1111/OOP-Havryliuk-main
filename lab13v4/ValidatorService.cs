using System.Globalization;
using System.Text.RegularExpressions;

namespace lab13v4;

public class ValidatorService
{
    private static readonly Regex EmailRegex =
        new Regex(@"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);

    public void ValidateProduct(Product product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(product.Name))
        {
            errors.Add("Назва продукту не може бути порожньою");
        }
        else if (product.Name.Length > 50)
        {
            errors.Add("Назва продукту не може бути довшою за 50 символів");
        }

        if (product.Price <= 0)
        {
            errors.Add("Ціна повинна бути більшою за 0");
        }

        if (product.Quantity < 0)
        {
            errors.Add("Кількість не може бути від'ємною");
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }
    }

    public Result<bool> IsEmailValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<bool>.Failure("Email не може бути порожнім");
        }

        if (!EmailRegex.IsMatch(email))
        {
            return Result<bool>.Failure($"Email \"{email}\" не відповідає формату name@domain.com");
        }

        return Result<bool>.Success(true);
    }

    public Result<decimal> ParsePrice(string input)
    {
        try
        {
            decimal price = decimal.Parse(input, NumberStyles.Number, CultureInfo.InvariantCulture);

            if (price <= 0)
            {
                return Result<decimal>.Failure("Ціна повинна бути більшою за 0");
            }

            return Result<decimal>.Success(price);
        }
        catch (FormatException ex)
        {
            return Result<decimal>.Failure(ex);
        }
        catch (OverflowException ex)
        {
            return Result<decimal>.Failure(ex);
        }
    }
}
