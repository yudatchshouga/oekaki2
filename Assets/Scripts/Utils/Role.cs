public enum Role
{
    Questioner,
    Answerer,
    None
}

// 拡張メソッド
public static class RoleExtension
{
    public static Role toRole(this string str)
    {
        return str switch
        {
            "Questioner" => Role.Questioner,
            "Answerer" => Role.Answerer,
            _ => Role.None
        };
    }
}