using System;

namespace ObjectPrinting.Tests
{
	public class Person
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public double Height { get; set; }
		public int Age { get; set; }
	    public Person Parent { get; set; }
	    public object[] Bla { get; set; }
	    public int[][] TwoDimensionalArr { get; set; }

    }
}