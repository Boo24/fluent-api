using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
	[TestFixture]
	public class ObjectPrinterAcceptanceTests
	{
	    private Person person;

	    [SetUp]
	    public void SetUp() => person = new Person { Name = "Alex", Age = 19 };

	    [Test]
	    public void StringWithoutIntProperties_WhenExcludeIntType()
	    {
	        var a = 1;
	        var b = person.GetType().IsValueType;
	        var printer = ObjectPrinter.For<Person>().ExcludeType<int>();
            var actualResult = printer.PrintToString(person);
	        var expectedResult = "Person\r\n	Id = Guid\r\n	Name = Alex\r\n	Height = 0\r\n";
            actualResult.ShouldBeEquivalentTo(expectedResult);

	    }
	    [Test]
	    public void StringWithSetSerializationMethodForInt()
	    {
	        var printer = ObjectPrinter.For<Person>().SetTypeSerialization(ctx => ctx.ForTarget<int>().Use(x=>x.ToString()+"!"));
	        var actualResult = printer.PrintToString(person);
	        var expectedResult = "Person\r\n	Id = Guid\r\n	Name = Alex\r\n	Height = 0\r\n	Age = 19!\r\n";
	        actualResult.ShouldBeEquivalentTo(expectedResult);
	    }

	    [Test]
	    public void StringWithSpecificCulture_WhenSetSpecificCultureForDouble()
	    {
            var person = new Person { Name = "Alex", Age = 19, Height = 190.4 };
            var printer = ObjectPrinter.For<Person>()
                .SetTypeSerialization(x=>x.ForTarget<double>().UsingCulture(CultureInfo.CreateSpecificCulture("de-DE")));
            var actualResult = printer.PrintToString(person);
            var expectedResult = "Person\r\n	Id = Guid\r\n	Name = Alex\r\n	Height = 190,4\r\n	Age = 19\r\n";
            actualResult.ShouldBeEquivalentTo(expectedResult);
        }

	    [Test]
	    public void StringWithSetSerializationMethodForName()
	    {
	        var printer = ObjectPrinter.For<Person>().SetPropertySerialization(ctx => ctx.ForTarget(p => p.Name).Use(x => x.ToString()+"!"));
            var actualResult = printer.PrintToString(person);
	        var expectedResult = "Person\r\n	Id = Guid\r\n	Name = Alex!\r\n	Height = 0\r\n	Age = 19\r\n";
	        actualResult.ShouldBeEquivalentTo(expectedResult);
        }

        [Test]
	    public void CutString()
	    {
	        var printer = ObjectPrinter.For<Person>().SetPropertySerialization(ctx => ctx.ForTarget(x=>x.Name).Cut(2));
	        var actualResult = printer.PrintToString(person);
            var expectedResult = "Person\r\n	Id = Guid\r\n	Name = Al\r\n	Height = 0\r\n	Age = 19\r\n";
	        actualResult.ShouldBeEquivalentTo(expectedResult);
        }

	    [Test]
	    public void StringWithoutExcludeProrepty_WhenExcludeName()
	    {
	        var printer = ObjectPrinter.For<Person>().ExcludeProperty(p => p.Name);
	        var actualResult = printer.PrintToString(person);
	        var expectedResult = "Person\r\n	Id = Guid\r\n	Height = 0\r\n	Age = 19\r\n";
	        actualResult.ShouldBeEquivalentTo(expectedResult);
        }

	    [Test]
	    public void StringToPrint_WhenDefaultSerializeWithConfiguration()
	    {
	        var actualResult = person.PrintToString(p => p.ExcludeType<int>());
	        var expectedResult = "Person\r\n	Id = Guid\r\n	Name = Alex\r\n	Height = 0\r\n";
	        actualResult.ShouldBeEquivalentTo(expectedResult);

        }
	    [Test]
	    public void Demo()
	    {
	        var person = new Person { Name = "Alex", Age = 19 };
	        var printer = ObjectPrinter.For<Person>()
	            //1. Исключить из сериализации свойства определенного типа
	            .ExcludeType<int>()
	            //2. Указать альтернативный способ сериализации для определенного типа
                .SetTypeSerialization(o=> o.ForTarget<int>().Use(x=>x.ToString()))
                ////3. Для числовых типов указать культуру;
	            .SetTypeSerialization(x => x.ForTarget<double>().UsingCulture(CultureInfo.CreateSpecificCulture("de-DE")))
	            ////4. Настроить сериализацию конкретного свойства
	            .SetPropertySerialization(o => o.ForTarget(p=>p.Age).Use(x=>x.ToString()))
	            ////5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
	            .SetPropertySerialization(ctx => ctx.ForTarget(x=>x.Name).Cut(7))
	            ////6. Исключить из сериализации конкретного свойства
	            .ExcludeProperty(p => p.Name);
	        string s1 = printer.PrintToString(person);

	        //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию		
	        var s2 = person.PrintToString();
	        //8. ...с конфигурированием
	        var s3 = person.PrintToString(p => p.ExcludeType<int>());
	    }


    }

    public class Progmam
    {
        public static void Main(string[] args)
        {
            
        }
    }
}