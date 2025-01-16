namespace GoldenRaspberryAwards.Core.Interfaces;

public interface IEntityValidator<T>
{
    List<string> Validate(T entity);
}
