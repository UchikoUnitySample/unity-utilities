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
    public void OptionNoneTest()
    {
		Option<int> op = Option<int>.None();
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
    }
}
