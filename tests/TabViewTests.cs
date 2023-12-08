using System.IO;
using System.Linq;
using Terminal.Gui;
using TerminalGuiDesigner;
using TerminalGuiDesigner.FromCode;
using TerminalGuiDesigner.Operations;
using TerminalGuiDesigner.Operations.TabOperations;
using TerminalGuiDesigner.ToCode;

namespace UnitTests;

[TestFixture]
internal class TabViewTests : Tests
{
    [Test]
    public void TestRoundTrip_PreserveTabs()
    {
        TabView tabIn = RoundTrip<Dialog, TabView>(
            (d, t) =>
            ClassicAssert.IsNotEmpty(t.Tabs, "Expected default TabView created by ViewFactory to have some placeholder Tabs"),
            out TabView tabOut);

        ClassicAssert.AreEqual(2, tabIn.Tabs.Count());

        ClassicAssert.AreEqual("Tab1", tabIn.Tabs.ElementAt(0).Text);
        ClassicAssert.AreEqual("Tab2", tabIn.Tabs.ElementAt(1).Text);
    }

    /// <summary>
    /// Creates a Dialog with a <see cref="TabView"/> in it.  Returns the <see cref="Design"/>
    /// </summary>
    /// <returns></returns>
    private Design GetTabView()
    {
        var viewToCode = new ViewToCode();

        var file = new FileInfo("TestGetTabView.cs");
        var designOut = viewToCode.GenerateNewView(file, "YourNamespace", typeof(Dialog));

        var tvOut = ViewFactory.Create<TabView>( );

        OperationManager.Instance.Do(new AddViewOperation(tvOut, designOut, "myTabview"));

        return (Design)tvOut.Data;
    }

    [Test]
    public void TestChangeTabViewOrder_MoveTabLeft()
    {
        var d = this.GetTabView();
        var tv = (TabView)d.View;

        ClassicAssert.AreEqual("Tab1", tv.Tabs.ElementAt(0).Text);
        ClassicAssert.AreEqual("Tab2", tv.Tabs.ElementAt(1).Text);

        // Select Tab1
        tv.SelectedTab = tv.Tabs.First();

        // try to move tab 1 left
        var cmd = new MoveTabOperation(d, tv.SelectedTab, -1);
        ClassicAssert.IsFalse(cmd.Do(), "Expected not to be able to move tab left because selected is the first");

        // Select Tab2
        tv.SelectedTab = tv.Tabs.Last();

        ClassicAssert.AreEqual(tv.SelectedTab.Text, "Tab2", "Tab2 should be selected before operation is applied");

        // try to move tab 2 left
        cmd = new MoveTabOperation(d, tv.SelectedTab, -1);
        ClassicAssert.IsFalse(cmd.IsImpossible);
        ClassicAssert.IsTrue(cmd.Do());

        ClassicAssert.AreEqual(tv.SelectedTab.Text, "Tab2", "Tab2 should still be selected after operation is applied");

        // tabs should now be in reverse order
        ClassicAssert.AreEqual("Tab2", tv.Tabs.ElementAt(0).Text);
        ClassicAssert.AreEqual("Tab1", tv.Tabs.ElementAt(1).Text);

        cmd.Undo();

        // undoing command should revert to original tab order
        ClassicAssert.AreEqual("Tab1", tv.Tabs.ElementAt(0).Text);
        ClassicAssert.AreEqual("Tab2", tv.Tabs.ElementAt(1).Text);
    }

    [Test]
    public void TestRemoveTabOperation()
    {
        var d = this.GetTabView();
        var tv = (TabView)d.View;

        ClassicAssert.AreEqual(2, tv.Tabs.Count);
        ClassicAssert.AreEqual("Tab1", tv.Tabs.ElementAt(0).Text);
        ClassicAssert.AreEqual("Tab2", tv.Tabs.ElementAt(1).Text);

        // Select Tab1
        tv.SelectedTab = tv.Tabs.First();

        // try to remove the first tab
        ClassicAssert.IsTrue(OperationManager.Instance.Do(new RemoveTabOperation(d, tv.SelectedTab)));

        ClassicAssert.AreEqual(1, tv.Tabs.Count);
        ClassicAssert.AreEqual("Tab2", tv.Tabs.ElementAt(0).Text);

        // remove the last tab (tab2)
        ClassicAssert.IsTrue(OperationManager.Instance.Do(new RemoveTabOperation(d, tv.SelectedTab)));
        ClassicAssert.IsEmpty(tv.Tabs);

        OperationManager.Instance.Undo();

        ClassicAssert.AreEqual(1, tv.Tabs.Count);
        ClassicAssert.AreEqual("Tab2", tv.Tabs.ElementAt(0).Text);

        OperationManager.Instance.Redo();
        ClassicAssert.IsEmpty(tv.Tabs);

        // undo removing both
        OperationManager.Instance.Undo();
        OperationManager.Instance.Undo();

        ClassicAssert.AreEqual(2, tv.Tabs.Count);
        ClassicAssert.AreEqual("Tab1", tv.Tabs.ElementAt(0).Text);
        ClassicAssert.AreEqual("Tab2", tv.Tabs.ElementAt(1).Text);
    }

    [Test]
    public void TestRoundTrip_DuplicateTabNames()
    {
        var viewToCode = new ViewToCode();

        var file = new FileInfo("TestRoundTrip_DuplicateTabNames.cs");
        var designOut = viewToCode.GenerateNewView(file, "YourNamespace", typeof(Dialog));

        var tvOut = ViewFactory.Create<TabView>( );

        OperationManager.Instance.Do(new AddViewOperation(tvOut, designOut, "myTabview"));

        // Give both tabs the same name
        tvOut.Tabs.ElementAt(0).Text = "MyTab";
        tvOut.Tabs.ElementAt(1).Text = "MyTab";

        viewToCode.GenerateDesignerCs(designOut, typeof(Dialog));

        var tabOut = designOut.View.GetActualSubviews().OfType<TabView>().Single();

        var codeToView = new CodeToView(designOut.SourceCode);
        var designBackIn = codeToView.CreateInstance();

        var tabIn = designBackIn.View.GetActualSubviews().OfType<TabView>().Single();

        ClassicAssert.AreEqual(2, tabIn.Tabs.Count());

        ClassicAssert.AreEqual("MyTab", tabIn.Tabs.ElementAt(0).Text);
        ClassicAssert.AreEqual("MyTab", tabIn.Tabs.ElementAt(1).Text);
    }

    [Test]
    public void TestAddingSubcontrolToTab()
    {
        var viewToCode = new ViewToCode();

        var file = new FileInfo("TestAddingSubcontrolToTab.cs");
        var designOut = viewToCode.GenerateNewView(file, "YourNamespace", typeof(Dialog));

        var tvOut = ViewFactory.Create<TabView>( );

        OperationManager.Instance.Do(new AddViewOperation(tvOut, designOut, "myTabview"));

        var label = ViewFactory.Create<Label>( );

        OperationManager.Instance.Do(new AddViewOperation(label, (Design)tvOut.Data, "myLabel"));
        ClassicAssert.Contains(label, tvOut.SelectedTab.View.Subviews.ToArray(), "Expected currently selected tab to have the new label but it did not");

        viewToCode.GenerateDesignerCs(designOut, typeof(Dialog));

        var codeToView = new CodeToView(designOut.SourceCode);
        var designBackIn = codeToView.CreateInstance();

        var tabIn = designBackIn.View.GetActualSubviews().OfType<TabView>().Single();
        var tabInLabel = tabIn.SelectedTab.View.Subviews.Single();

        ClassicAssert.AreEqual(label.Text, tabInLabel.Text);
    }

    [Test]
    public void TestGetAllDesigns_TabView()
    {
        var tv = new TabView();

        var source = new SourceCodeFile(new FileInfo("yarg.cs"));

        var lbl1 = new Design(source, "lbl1", new Label("fff"));
        lbl1.View.Data = lbl1;

        var lbl2 = new Design(source, "lbl2", new Label("ddd"));
        lbl2.View.Data = lbl2;

        tv.AddTab(new Tab("Yay", lbl1.View), true);
        tv.AddTab(new Tab("Yay", lbl2.View), false);

        var tvDesign = new Design(source, "tv", tv);

        ClassicAssert.Contains(tvDesign, tvDesign.GetAllDesigns().ToArray());
        ClassicAssert.Contains(lbl1, tvDesign.GetAllDesigns().ToArray());
        ClassicAssert.Contains(lbl2, tvDesign.GetAllDesigns().ToArray());

        ClassicAssert.AreEqual(3, tvDesign.GetAllDesigns().Count(), $"Expected only 3 Designs but they were {string.Join(",", tvDesign.GetAllDesigns())}");
    }

    [Test]
    public void TabView_IsBorderless_DependsOnShowBorder()
    {
        var inst = new TabView();

        ClassicAssert.IsTrue(inst.Style.ShowBorder);
        ClassicAssert.False(inst.IsBorderlessContainerView());

        inst.Style.ShowBorder = false;

        ClassicAssert.True(inst.IsBorderlessContainerView());
    }

    [Test]
    public void TabView_IsBorderless_DependsOnTabsOnBottom()
    {
        var inst = new TabView();

        ClassicAssert.IsFalse(inst.Style.TabsOnBottom);
        ClassicAssert.False(inst.IsBorderlessContainerView());

        inst.Style.TabsOnBottom = true;

        ClassicAssert.True(inst.IsBorderlessContainerView());
    }
}