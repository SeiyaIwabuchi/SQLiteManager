using System;
using Gtk;
using System.Data.SQLite;
using System.Text;
using System.Linq;
using Glade;

public partial class MainWindow : Gtk.Window
{

    test t1 = new test(1);
    SQLiteConnectionStringBuilder SQLiteConnectionSb;
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        GtkWi
        SQLiteConnectionSb = new SQLiteConnectionStringBuilder { DataSource = ":memory:" };
        OutputTextView.overriteText(sqlCommonExecuter("select sqlite_version()")+"\n");
        OutputTextView.postscriptText("Hello SQLite!\n");
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void exec(object sender, EventArgs e)
    {
        String query = InputTextView.getText();
        try
        {
            OutputTextView.overriteText(sqlCommonExecuter(query));
            OutputTextView.postscriptText("Query executed.\n");
        }
        catch(Exception exc)
        {
            OutputTextView.postscriptText(exc.Message);
        }

    }

    String sqlCommonExecuter(String query)
    {
    String dump;
        using (SQLiteConnection cn = new SQLiteConnection(SQLiteConnectionSb.ToString()))
        {
            cn.Open();
            using (SQLiteCommand cmd = new SQLiteCommand(cn))
            {
                cmd.CommandText = query;
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    dump = reader.DumpQuery();
                }
            }
        }
        return dump;
    }

    protected void hdConnectDB(object sender, EventArgs e)
    {
        String dbName = DbNameInput.Buffer.Text;
        SQLiteConnectionSb = new SQLiteConnectionStringBuilder { DataSource = dbName };
        OutputTextView.overriteText("Connection successful!\n");
    }
}

static class MyTextView
{
    public static void overriteText(this TextView tv,String str)
    {
        tv.Buffer.Text = str;
    }

    public static void postscriptText(this TextView tv,String str)
    {
        tv.Buffer.Text = tv.Buffer.Text + str;
    }

    public static void clearText(this TextView tv,String str)
    {
        tv.Buffer.Text = "";
    }
    public static String getText(this TextView tv)
    {
        return tv.Buffer.Text;
    }

    public static string DumpQuery(this SQLiteDataReader reader)
    {
        var i = 0;
        var sb = new StringBuilder();
        while (reader.Read())
        {
            if (i == 0)
            {
                sb.AppendLine(string.Join("\t", reader.GetValues().AllKeys));
                sb.AppendLine(new string('=', 8 * reader.FieldCount));
            }
            sb.AppendLine(string.Join("\t", Enumerable.Range(0, reader.FieldCount).Select(x => reader.GetValue(x))));
            i++;
        }

        return sb.ToString();
    }
}

class test
{
    private int a;
    public test(int num)
    {
        a = num;
    }
    public int getNum()
    {
        return this.a;
    }
    public void setNum(int num)
    {
        this.a = num;
    }
}