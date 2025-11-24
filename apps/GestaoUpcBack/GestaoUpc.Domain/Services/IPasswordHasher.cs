namespace GestaoUpc.Domain.Services;

/// <summary>
/// Interface para serviço de hash de senhas
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Gera um hash da senha fornecida
    /// </summary>
    /// <param name="password">Senha em texto plano</param>
    /// <returns>Hash da senha</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifica se a senha fornecida corresponde ao hash armazenado
    /// </summary>
    /// <param name="password">Senha em texto plano</param>
    /// <param name="hashedPassword">Hash da senha armazenado</param>
    /// <returns>True se a senha corresponder ao hash, caso contrário False</returns>
    bool VerifyPassword(string password, string hashedPassword);
}

