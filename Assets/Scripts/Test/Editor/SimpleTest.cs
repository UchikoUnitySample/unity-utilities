using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;
using Hatool;

public class SimpleTest {

    [Test]
    public void EditorTest()
    {
        //Arrange
        var gameObject = new GameObject();

        //Act
        //Try to rename the GameObject
        var newGameObjectName = "My game object";
        gameObject.name = newGameObjectName;

        //Assert
        //The object has a new name
        Assert.AreEqual(newGameObjectName, gameObject.name);
    }

	[Test]
	public void StringExtensionTest()
	{
		Assert.AreEqual("a b", "{0} {1}".Fmt("a", "b"));
		Assert.True("".IsNullOrWhiteSpace());
		Assert.True(" ".IsNullOrWhiteSpace());
		Assert.False(" a ".IsNullOrWhiteSpace());
	}

	[Test]
	public void OptionSomeTest()
	{
		Option<int> op = new Some<int>(1);
		Assert.True(op.IsDefined);
		Assert.AreEqual(1, op.Get());
		Assert.AreEqual(new Some<int>(2), op.Map((a) => a+1));
		Assert.AreEqual(new Some<int>(2), op.FlatMap((a) => new Some<int>(a+1)));
		Assert.AreEqual(Option<int>.None(), op.FlatMap((a) => Option<int>.None()));
		Assert.AreEqual(1, op.GetOrElse(() => 2));
		Assert.AreEqual(1, op.GetOrElse(2));
		Assert.AreEqual(new Some<int>(1), op.OrElse(() => new Some<int>(1)));
		Assert.AreEqual(new Some<int>(1), op.OrElse(new Some<int>(1)));
		Assert.AreEqual(new List<int>(){1}, op.ToList());
		Assert.True(op.Equals(new Some<int>(1)));
		Assert.False(op.Equals(new Some<int>(2)));
		Assert.AreNotEqual(1, op.GetHashCode());
		Assert.AreEqual("Some(1)", op.ToString());
	}

    [Test]
    public void OptionNoneTest()
    {
		Option<int> op = Option<int>.None();
		Assert.False(op.IsDefined);
		Assert.Throws(typeof(NoneToGetException), delegate { op.Get(); });
		Assert.AreEqual(Option<int>.None(), op.Map((a) => 1));
		Assert.AreEqual(Option<int>.None(), op.FlatMap((a) => new Some<int>(1)));
		Assert.AreEqual(1, op.GetOrElse(() => 1));
		Assert.AreEqual(1, op.GetOrElse(1));
		Assert.AreEqual(new Some<int>(1), op.OrElse(new Some<int>(1)));
		Assert.AreEqual(new Some<int>(1), op.OrElse(() => new Some<int>(1)));
		Assert.AreEqual((new List<int>()).Count, op.ToList().Count);
		Assert.True(op.Equals(Option<int>.None()));
		Assert.False(op.Equals(new Some<int>(1)));
		Assert.AreEqual("None", op.ToString());
		Assert.AreEqual(0, op.GetHashCode());

		var i = 0;
		op.Foreach((a) => i++);
		Assert.AreEqual(0, i);
    }
}
