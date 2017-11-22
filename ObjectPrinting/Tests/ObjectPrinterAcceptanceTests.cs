using System.Globalization;
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
	        var printer = ObjectPrinter.For<Person>().ExcludeType<int>();
            var actualResult = printer.PrintToString(person);
	        var expectedResult = "Person\r\n	Id = Guid\r\n	Name = Alex\r\n	Height = 0\r\n";
            actualResult.ShouldBeEquivalentTo(expectedResult);

	    }
	    [Test]
	    public void StringWithSetSerializationMethodForInt()
	    {
	        var printer = ObjectPrinter.For<Person>().Printing<int>().Using(p => p.ToString() + "!");
	        var actualResult = printer.PrintToString(person);
	        var expectedResult = "Person\r\n	Id = Guid\r\n	Name = Alex\r\n	Height = 0\r\n	Age = 19!\r\n";
	        actualResult.ShouldBeEquivalentTo(expectedResult);
	    }

	    [Test]
	    public void StringWithSpecificCulture_WhetSetSpecificCultureForDouble()
	    {
	        var person = new Person { Name = "Alex", Age = 19, Height = 190.4};
	        var printer = ObjectPrinter.For<Person>().Printing<double>().Using(CultureInfo.CreateSpecificCulture("de-DE"));
	        var actualResult = printer.PrintToString(person);
	        var expectedResult = "Person\r\n	Id = Guid\r\n	Name = Alex\r\n	Height = 190,4\r\n	Age = 19\r\n";
	        actualResult.ShouldBeEquivalentTo(expectedResult);
        }

	    [Test]
	    public void StringWithSetSerializationMethodForName()
	    {
	        var printer = ObjectPrinter.For<Person>().Printing(p=>p.Name).Using(p => p.ToString() + "!");
	        var actualResult = printer.PrintToString(person);
	        var expectedResult = "Person\r\n	Id = Guid\r\n	Name = Alex!\r\n	Height = 0\r\n	Age = 19\r\n";
	        actualResult.ShouldBeEquivalentTo(expectedResult);
        }

        [Test]
	    public void CutString()
	    {
	        var printer = ObjectPrinter.For<Person>().Printing(p => p.Name).Cut(2);
	        var actualResult = printer.PrintToString(person);
            var expectedResult = "Person\r\n	Id = Guid\r\n	Name = Al\r\n	Height = 0\r\n	Age = 19\r\n";
	        actualResult.ShouldBeEquivalentTo(expectedResult);
        }

	    [Test]
	    public void StringWithoutExcludeProrepty_WhenExcludeName()
	    {
	        var printer = ObjectPrinter.For<Person>().Excluding(p => p.Name);
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
	            .Printing<int>()
	            .Using(p => p.ToString())
	            ////3. Для числовых типов указать культуру;
	            .Printing<double>()
	            .Using(CultureInfo.CurrentCulture)
	            ////4. Настроить сериализацию конкретного свойства
	            .Printing(p => p.Age)
	            .Using(p => p.ToString())
	            ////5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
	            .Printing(p => p.Name)
	            .Cut(10)
	            ////6. Исключить из сериализации конкретного свойства
	            .Excluding(p => p.Name);

	        string s1 = printer.PrintToString(person);

	        //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию		
	        //8. ...с конфигурированием
	    }


    }
}