using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services.Tests
{
    #nullable disable
    public class CollectionExtensionsTests
    {

        [Fact]
        public void SortedListCompareTest()
        {

            var list1 = new SortedList<int, MyClass>();
            list1.Add(1, new MyClass { Id = 1, Name = "One" });
            list1.Add(2, new MyClass { Id = 2, Name = "Two" });
            list1.Add(3, new MyClass { Id = 3, Name = "Three" });

            var list2 = new SortedList<int, MyClass>();
            list2.Add(2, new MyClass { Id = 2, Name = "Two" });
            list2.Add(3, new MyClass { Id = 3, Name = "ModifiedThree" });
            list2.Add(4, new MyClass { Id = 4, Name = "Four" });

            var differences = list1.CompareTo(list2, item => item.Id);

            foreach (var difference in differences)
            {
                Console.WriteLine($"{difference.Value.Name} ({difference.Location})");
            }
        }

        
    }

    public class MyClass
    { 
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
