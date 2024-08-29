
using System.Data;

public class UserData
{
    private int uid;
    public int UID => uid;

    public string email;
    public string passwd { get; set; }

    public string name;

    public int EXP;

    public UserData(string email, string passwd, string name, int EXP)
    {
        this.email = email;
        this.passwd = passwd;
        this.name = name;
        this.EXP = EXP;
    }

    public UserData(DataRow row)
    {
        this.uid = int.Parse(row["uid"].ToString());
        this.email = row["email"].ToString();
        this.passwd = row["passwd"].ToString();
        this.name = row["name"].ToString();
        this.EXP = int.Parse(row["EXP"].ToString());
    }
}
