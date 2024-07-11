using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using eID.PUN.Contracts.Results;
using eID.PUN.Service.Database;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace eID.PUN.UnitTests.Generic;

public abstract class BaseTest
{
    protected static TestApplicationDbContext GetTestDbContext() =>
        new TestApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options
        );

    protected static void CheckServiceResult(ServiceResult serviceResult, HttpStatusCode statusCode)
    {
        Assert.That(serviceResult, Is.Not.Null);
        Assert.That(serviceResult.StatusCode, Is.EqualTo(statusCode));
    }

    protected static void CheckServiceResult<T>(ServiceResult<T> serviceResult, HttpStatusCode statusCode)
    {
        Assert.That(serviceResult, Is.Not.Null);
        if (statusCode < HttpStatusCode.BadRequest)
        {
            Assert.That(serviceResult.Result, Is.Not.Null);
            Assert.IsNull(serviceResult.Error);
            Assert.IsNull(serviceResult.Errors);
        }
        else
        {
            Assert.True(serviceResult.Errors != null || serviceResult.Errors != null);
        }
        Assert.That(serviceResult.StatusCode, Is.EqualTo(statusCode));
    }

    protected static T CreateInterface<T>(object propertyValues)
    {
        _ = propertyValues ?? throw new ArgumentNullException(nameof(propertyValues));
        var interfaceType = typeof(T);

        if (!interfaceType.IsInterface)
        {
            throw new ArgumentException($"Type: {interfaceType.FullName} is not an Interface");
        }

        // Create an instance of interface
        var typeBuilder = CreateTypeBuilder(interfaceType.Name, interfaceType);
        var dynamicType = typeBuilder.CreateType();
        var result = (T)Activator.CreateInstance(dynamicType);

        // Copy properties from the object to the interface object
        foreach (var property in propertyValues.GetType().GetProperties())
        {
            var interfaceProperty = result.GetType().GetProperty(property.Name);
            interfaceProperty.SetValue(result, property.GetValue(propertyValues));
        }

        return result;
    }

    private static TypeBuilder CreateTypeBuilder(string typeName, Type interfaceType)
    {
        var assemblyName = new AssemblyName(typeName);
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule(typeName);
        var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public);
        var propertyAttibutes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig;

        var properties = interfaceType
            .GetInterfaces().SelectMany(s => s.GetProperties())
            .Union(interfaceType.GetProperties());
        foreach (var property in properties)
        {
            var fieldBuilder = typeBuilder.DefineField("_" + property.Name, property.PropertyType, FieldAttributes.Private);
            var propertyBuilder = typeBuilder.DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, null);
            var getMethodBuilder = typeBuilder.DefineMethod("get_" + property.Name, propertyAttibutes, property.PropertyType, Type.EmptyTypes);
            var getIL = getMethodBuilder.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getIL.Emit(OpCodes.Ret);
            var setMethodBuilder = typeBuilder.DefineMethod("set_" + property.Name, propertyAttibutes, null, new Type[] { property.PropertyType });
            var setIL = setMethodBuilder.GetILGenerator();
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, fieldBuilder);
            setIL.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getMethodBuilder);
            propertyBuilder.SetSetMethod(setMethodBuilder);
        }

        typeBuilder.AddInterfaceImplementation(interfaceType);

        return typeBuilder;
    }
}
