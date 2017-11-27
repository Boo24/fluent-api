﻿using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        private Person person;

        [SetUp]
        public void SetUp() => person = new Person
        {
            Name = "Alex",
            Age = 19,
            Parent = new Person() {Name = "qwer"},
            Bla = new object[] {new[] {1, 2}, new[] {1, 2}}
        };

        [Test]
        public void StringWithoutIntProperties_WhenExcludeIntType()
        {
            var person = new Person {Name = "Alex", Age = 19, Parent = new Person() {Name = "qwer"}};
            var printer = ObjectPrinter.For<Person>().ExcludeType<int>();
            var actualResult = printer.PrintToString(person);
            var expectedResult = "Person\r\n" +
                                 "	Id = Guid\r\n" +
                                 "	Name = Alex\r\n" +
                                 "	Height = 0\r\n" +
                                 "	Parent = Person\r\n" +
                                 "		Id = Guid\r\n" +
                                 "		Name = qwer\r\n" +
                                 "		Height = 0\r\n" +
                                 "		Parent = null\r\n" +
                                 "		Bla = null\r\n" +
                                 "		TwoDimensionalArr = null\r\n" +
                                 "	Bla = null\r\n" +
                                 "	TwoDimensionalArr = null\r\n";
            actualResult.ShouldBeEquivalentTo(expectedResult);
        }

        [Test]
        public void StringWithSetSerializationMethodForInt()
        {
            var person = new Person
            {
                Name = "Alex",
                Age = 19,
                Parent = new Person() {Name = "qwer"},
                Bla = new object[] {new Person() {Age = 1}, 1, new[] {3, 4}}
            };
            var printer = ObjectPrinter.For<Person>().SetTypeSerialization(ctx => ctx.ForTarget<int>().Use(x => x.ToString() + "!"));
            var actualResult = printer.PrintToString(person);
            var expectedResult =
                "Person\r\n" +
                "	Id = Guid\r\n" +
                "	Name = Alex\r\n" +
                "	Height = 0\r\n" +
                "	Age = 19!\r\n" +
                "	Parent = Person\r\n" +
                "		Id = Guid\r\n" +
                "		Name = qwer\r\n" +
                "		Height = 0\r\n" +
                "		Age = 0!\r\n" +
                "		Parent = null\r\n" +
                "		Bla = null\r\n" +
                "		TwoDimensionalArr = null\r\n" +
                "	Bla = 	Object[]" +
                "		Person\r\n" +
                "			Id = Guid\r\n" +
                "			Name = null\r\n" +
                "			Height = 0\r\n" +
                "			Age = 1!\r\n" +
                "			Parent = null\r\n" +
                "			Bla = null\r\n" +
                "			TwoDimensionalArr = null\r\n" +
                "		1!" +
                "				Int32[]" +
                "			3!" +
                "			4!" +
                "	TwoDimensionalArr = null\r\n";
            actualResult.ShouldBeEquivalentTo(expectedResult);
        }

        [Test]
        public void StringWithSpecificCulture_WhenSetSpecificCultureForDouble()
        {
            var person = new Person {Name = "Alex", Age = 19, Height = 190.4};
            var printer = ObjectPrinter.For<Person>()
                .SetTypeSerialization(x =>
                    x.ForTarget<double>().UsingCulture(CultureInfo.CreateSpecificCulture("de-DE")));
            var actualResult = printer.PrintToString(person);
            var expectedResult = "Person\r\n" +
                                 "	Id = Guid\r\n" +
                                 "	Name = Alex\r\n" +
                                 "	Height = 190,4\r\n" +
                                 "	Age = 19\r\n" +
                                 "	Parent = null\r\n" +
                                 "	Bla = null\r\n" +
                                 "	TwoDimensionalArr = null\r\n";
            actualResult.ShouldBeEquivalentTo(expectedResult);
        }

        [Test]
        public void StringWithSetSerializationMethodForName()
        {
            var printer = ObjectPrinter.For<Person>()
                .SetPropertySerialization(ctx => ctx.ForTarget(p => p.Name).Use(x => x.ToString() + "!"));
            var actualResult = printer.PrintToString(person);
            var expectedResult = "Person\r\n" +
                                 "	Id = Guid\r\n" +
                                 "	Name = Alex!\r\n" +
                                 "	Height = 0\r\n" +
                                 "	Age = 19\r\n" +
                                 "	Parent = Person\r\n" +
                                 "		Id = Guid\r\n" +
                                 "		Name = qwer!\r\n" +
                                 "		Height = 0\r\n" +
                                 "		Age = 0\r\n" +
                                 "		Parent = null\r\n" +
                                 "		Bla = null\r\n" +
                                 "		TwoDimensionalArr = null\r\n" +
                                 "	Bla = 	Object[]" +
                                 "				Int32[]" +
                                 "			1\r\n" +
                                 "			2\r\n" +
                                 "				Int32[]" +
                                 "			1\r\n" +
                                 "			2\r\n" +
                                 "	TwoDimensionalArr = null\r\n";
            actualResult.ShouldBeEquivalentTo(expectedResult);
        }

        [Test]
        public void CutString()
        {
            var printer = ObjectPrinter.For<Person>()
                .SetPropertySerialization(ctx => ctx.ForTarget(x => x.Name).Cut(2));
            var actualResult = printer.PrintToString(person);
            var expectedResult = "Person\r\n" +
                                 "	Id = Guid\r\n" +
                                 "	Name = Al\r\n" +
                                 "	Height = 0\r\n" +
                                 "	Age = 19\r\n" +
                                 "	Parent = Person\r\n" +
                                 "		Id = Guid\r\n" +
                                 "		Name = qw\r\n" +
                                 "		Height = 0\r\n" +
                                 "		Age = 0\r\n" +
                                 "		Parent = null\r\n" +
                                 "		Bla = null\r\n" +
                                 "		TwoDimensionalArr = null\r\n" +
                                 "	Bla = 	Object[]" +
                                 "				Int32[]" +
                                 "			1\r\n" +
                                 "			2\r\n" +
                                 "				Int32[]" +
                                 "			1\r\n" +
                                 "			2\r\n" +
                                 "	TwoDimensionalArr = null\r\n";
            actualResult.ShouldBeEquivalentTo(expectedResult);
        }

        [Test]
        public void StringWithoutExcludeProrepty_WhenExcludeName()
        {
            var printer = ObjectPrinter.For<Person>().ExcludeProperty(p => p.Name);
            var actualResult = printer.PrintToString(person);
            var expectedResult = "Person\r\n" +
                                 "	Id = Guid\r\n" +
                                 "	Height = 0\r\n" +
                                 "	Age = 19\r\n" +
                                 "	Parent = Person\r\n" +
                                 "		Id = Guid\r\n" +
                                 "		Height = 0\r\n" +
                                 "		Age = 0\r\n" +
                                 "		Parent = null\r\n" +
                                 "		Bla = null\r\n" +
                                 "		TwoDimensionalArr = null\r\n" +
                                 "	Bla = 	Object[]" +
                                 "				Int32[]" +
                                 "			1\r\n" +
                                 "			2\r\n" +
                                 "				Int32[]" +
                                 "			1\r\n" +
                                 "			2\r\n" +
                                 "	TwoDimensionalArr = null\r\n";
            actualResult.ShouldBeEquivalentTo(expectedResult);
        }

        [Test]
        public void StringToPrint_WhenDefaultSerializeWithConfiguration()
        {
            var actualResult = person.PrintToString(p => p.ExcludeType<int>());
            var expectedResult = "Person\r\n" +
                                 "	Id = Guid\r\n" +
                                 "	Name = Alex\r\n" +
                                 "	Height = 0\r\n" +
                                 "	Parent = Person\r\n" +
                                 "		Id = Guid\r\n" +
                                 "		Name = qwer\r\n" +
                                 "		Height = 0\r\n" +
                                 "		Parent = null\r\n" +
                                 "		Bla = null\r\n" +
                                 "		TwoDimensionalArr = null\r\n" +
                                 "	Bla = 	Object[]" +
                                 "				Int32[]" +
                                 "				Int32[]" +
                                 "	TwoDimensionalArr = null\r\n";
            actualResult.ShouldBeEquivalentTo(expectedResult);
        }

        [Test]
        public void Demo()
        {
            var person = new Person {Name = "Alex", Age = 19};
            var printer = ObjectPrinter.For<Person>()
                //1. Исключить из сериализации свойства определенного типа
                .ExcludeType<int>()
                //2. Указать альтернативный способ сериализации для определенного типа
                .SetTypeSerialization(o => o.ForTarget<int>().Use(x => x.ToString()))
                ////3. Для числовых типов указать культуру;
                .SetTypeSerialization(x =>
                    x.ForTarget<double>().UsingCulture(CultureInfo.CreateSpecificCulture("de-DE")))
                ////4. Настроить сериализацию конкретного свойства
                .SetPropertySerialization(o => o.ForTarget(p => p.Age).Use(x => x.ToString()))
                ////5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                .SetPropertySerialization(ctx => ctx.ForTarget(x => x.Name).Cut(7))
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