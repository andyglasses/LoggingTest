namespace LoggingTest;

public class DemoStackGenerator
{
    public void DoAction()
    {
        var innerGen = new DemoStackGeneratorInner();
        innerGen.DoAction();
    }
}

public class DemoStackGeneratorInner
{
    public void DoAction()
    {
        try
        {
            var innerGen = new DemoStackGeneratorInnerInner();
            innerGen.DoAction();
        }
        catch (Exception e)
        {
            var exception = new Exception("See Inner Exception", e);
            exception.Data.Add("Custom Outer Data", "Custom Outer Data Value");
            throw exception;
        }
    }
}

public class DemoStackGeneratorInnerInner
{
    public void DoAction()
    {
        var innerGen = new DemoStackGeneratorInnerInnerInner();
        innerGen.DoAction();
    }
}

public class DemoStackGeneratorInnerInnerInner
{
    public void DoAction()
    {
        var exception = new Exception("Inner Error");
        exception.Data.Add("Custom Data", "Custom Data Value");
        throw exception;
    }
}

