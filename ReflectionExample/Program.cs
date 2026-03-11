using System.Reflection;
using ReflectionExample;

namespace Program
{

    //This is an example program of how to add to a readonly static dictionary with System.Reflection
    //Here is a really good tutorial for beginners that I used to learn a bit about reflection:
    //https://dotnetcademy.net/Learn/4/Pages/1

    static class Program
    {
        const string ExampleTypeName = "ReflectionExample.ExampleClass"; //the type name needs to be complete, with namespace included
        public static void Main()
        {
            ReadDictionaryBefore();

            Type? type = FindTypeInAssemblies(); //first you need a Type object
            if (type != null)
            {
                FindDictionaryField(type); //then... you access the Type object
            }
            else
                Console.WriteLine($"Could not find {ExampleTypeName} in loaded assemblies!");

            ReadDictionaryAfter();
        }


        static Type? FindTypeInAssemblies()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies(); //first we need to get all the loaded assemblies, which has all the types. easily done with this method
            foreach (Assembly assembly in assemblies) //then we foreach them
            {
                Type? type = assembly.GetType(ExampleTypeName); //and this part is super easy - we just run the assembly's gettype() methods with the type name as a parameter
                if (type != null)
                    return type;
            }
            return null;
        }


        static void FindDictionaryField(Type type)
        {
            //once we have the type, all we have to do is run the Type object's GetField method, with the field's full name
            FieldInfo? field = type.GetField("ExampleDictionary", BindingFlags.Static | BindingFlags.Public);

            //bindingFlags seem complicated at first but are not, its simple: we provide info about the field
            //in this case the field is static and public. if it an instance and private field, we would use BindingFlags.Instance | BindingFlags.Private
            //you dont need to worry about what | does right now

            if (field != null)
            {

                //so once we have the field, that's not enough, its just "Field Info", information about the field
                //GetValue returns the actual field value - for reference types, it will return a reference to the actual object that you can now access and modify
                object? dictionary = field.GetValue(null);

                //GetValue takes an "object" parameter, which would normally be an instance of the class who's field you are trying to modify
                //(IE. it would be an instance of ExampleClass, where our Dictionary field is)
                //however, because our field is static, it does not require an instance to modify, and our parameter should be null

                //once we have the actual object, we need to ask reflection to give us the method we want for it
                MethodInfo? addMethod = dictionary?.GetType().GetMethod("Add", new Type[] { typeof(string), typeof(int) });

                //so we need to get the type of the object that was returned from field.GetValue
                //GetType() returns a Type object for our local variable object? dictionary
                //GetMethod() returns the method information from our Type object

                //Our first parameter "Add" is the name, and our second parameter is a Type[] array that matches the parameters of the method you want
                //So we just match the parameters that you would normally see in Dictionary.Add, obviously replacing generic types with our types

                // public void Add(TKey key, TValue value)
                // {
                //     bool modified = TryInsert(key, value, InsertionBehavior.ThrowOnExisting);
                //     Debug.Assert(modified); // If there was an existing key and the Add failed, an exception will already have been thrown.
                // }


                //MethodInfo has an "Invoke" method that will invoke the method you got from the Type object

                //To invoke the method on the dictionary object and modify it, we need to pass the dictionary object as a parameter
                addMethod?.Invoke(dictionary, new object[] { "MyReflectedKey", 29101 });
                //Then, you need to make an Object[] array, and you need to fill it with values that match the parameters of the method you retrieved
                //So in effect, what's really happening here is dictionary.Add("MyReflectedKey", 29101);
            }
        }

    
    //CONSOLE OUTPUT:

// This is our dictionary before being accessed via reflection:

// ExampleKeyA, 42184
// ExampleKeyB, 29419054

// This is our dictionary after being accessed via reflection:

// ExampleKeyA, 42184
// ExampleKeyB, 29419054
// MyReflectedKey, 29101



        //Unimportant for learning, this is just for my console display
        static void ReadDictionaryBefore()
        {
            Console.WriteLine("This is our dictionary before being accessed via reflection:\n");
            foreach (var obj in ExampleClass.ExampleDictionary)
                Console.WriteLine($"{obj.Key}, {obj.Value}");
        }

        static void ReadDictionaryAfter()
        {
            Console.WriteLine("\nThis is our dictionary after being accessed via reflection:\n");
            foreach (var obj in ExampleClass.ExampleDictionary)
                Console.WriteLine($"{obj.Key}, {obj.Value}");
        }
    }
}

namespace ReflectionExample
{
    class ExampleClass
    {
        //These are the same access modifiers from Qudmoji
        //Actually qudmoji is not readonly, but it doesnt matter whether or not it is readonly
        //when using .Add
        public static readonly Dictionary<string, int> ExampleDictionary = new()
        {
            {"ExampleKeyA", 42184},
            {"ExampleKeyB", 29419054}
        };
    }
}

