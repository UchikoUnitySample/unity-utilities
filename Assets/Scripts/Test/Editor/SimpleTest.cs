using UnityEngine;
using UnityEditor;
using NUnit.Framework;

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
}
