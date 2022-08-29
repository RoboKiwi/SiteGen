namespace SiteGen.Tests.UnitTests.Models.Hierarchy;

public class TreeInfoTests
{
    [Fact]
    public void Tree_property()
    {
        Category category = new Category();
            
        Assert.NotNull(category.Tree);
        Assert.Null(category.Tree.Parent);
        Assert.NotNull(category.Tree.Children);
        Assert.Equal(category, category.Tree.Children.Parent);
        Assert.Equal(0, category.Tree.LeftValue);
        Assert.Equal(0, category.Tree.RightValue);
        Assert.Equal(false, category.Tree.IsDirty);
    }

    [Fact]
    public void Gets_marked_dirty_if_left_value_is_changed()
    {
        var category = new Category("Category1");
        Assert.False(category.Tree.IsDirty);

        category.Tree.LeftValue = 1;
        Assert.True(category.Tree.IsDirty);
    }

    [Fact]
    public void Gets_marked_dirty_if_parent_is_changed()
    {
        var category = new Category("Category1");
        Assert.False(category.Tree.IsDirty);

        category.Tree.Parent = new Category("Category2");
        Assert.True(category.Tree.IsDirty);
    }

    [Fact]
    public void Gets_marked_dirty_if_right_value_is_changed()
    {
        var category = new Category("Category1");
        Assert.False(category.Tree.IsDirty);

        category.Tree.RightValue = 1;
        Assert.True(category.Tree.IsDirty);
    }

    [Fact]
    public void Setting_parent_will_remove_the_child_from_its_previous_parents_children()
    {
        var parent = new Category("Parent");
        var parent2 = new Category("Parent2");
        var child = new Category("Child");

        // Add the child to the initial parent
        parent.Tree.Children.Add(child);
        Assert.Equal(child.Tree.Parent, parent);
        Assert.Equal(1, parent.Tree.Children.Count);

        // Now we'll change the child's parent
        child.Tree.Parent = parent2;

        // Child should have been removed from previous parent's children
        Assert.Equal(0, parent.Tree.Children.Count);
    }

    [Fact]
    public void Setting_parent_will_add_the_child_to_its_new_parents_children()
    {
        var parent = new Category("Parent");
        var child = new Category("Child");

        Assert.Equal(0, parent.Tree.Children.Count);

        // Add the child by setting the parent
        child.Tree.Parent = parent;

        Assert.Equal(child.Tree.Parent, parent);
        Assert.Equal(1, parent.Tree.Children.Count);
    }

    /// <summary>
    /// When you set a node's Parent, it will automatically update the Children collections
    /// of its previous and new Parent. You can also do the same thing by directly adding
    /// the node to the new Parent's Children. This method asserts that we don't inadvertently
    /// create duplicate entries of the child in the new Parent's Children collection
    /// when manipulating these properties.
    /// </summary>
    [Fact]
    public void Adding_child_by_setting_parent_or_adding_to_children_will_not_add_duplicates()
    {
        var parent = new Category("Parent");
        var child = new Category("Child");

        Assert.Equal(0, parent.Tree.Children.Count);

        child.Tree.Parent = parent;
        parent.Tree.Children.Add(child);

        Assert.Equal(1, parent.Tree.Children.Count);
    }
}