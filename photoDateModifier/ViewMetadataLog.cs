using BrightIdeasSoftware;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.ListView;

// Caution for NeGet ObjectListView:
//  To add ObjectListView to the toolbox, you can follow these steps: right - click on the toolbox,
//  Select "Choose Items", and then click the "Browse" button in the "Choose Toolbox Items" dialog.
//  Navigate to the ObjectListView.dll file, which should be located in the "packages" folder within
//  your Visual Studio solution directory. Select the ObjectListView.dll file and click "OK".
//  This should add the ObjectListView control to your toolbox.
namespace photoDateModifier
{
    public partial class ViewMetadataLog : Form
    {
        private List<MetadataModifyLog> metadataLogList = new List<MetadataModifyLog>();

        public ViewMetadataLog(string jasonFileName)
        {
            InitializeComponent();

            m_fastObjectListView.FullRowSelect = true;
            m_fastObjectListView.ShowFilterMenuOnRightClick = true;
            m_fastObjectListView.UseAlternatingBackColors = true;
            m_fastObjectListView.RetrieveVirtualItem += m_ObjectListView_RetrieveVirtualItem;

            var jsonString = File.ReadAllText(jasonFileName);
            var jsonLoadList = JsonConvert.DeserializeObject<SortedList<int, MetadataModifyLog>>(jsonString);
            var metadataLogList = jsonLoadList.Values.ToList();

            var columns = typeof(MetadataModifyLog).GetProperties()
                .Select(prop => new OLVColumn(prop.Name, prop.Name))
                .ToList();
            m_fastObjectListView.AllColumns.AddRange(columns);
            m_fastObjectListView.Columns.AddRange(columns.ToArray());
            m_fastObjectListView.VirtualListSize = metadataLogList.Count;
            m_fastObjectListView.VirtualListDataSource.SetObjects(metadataLogList);
            AllowFitColumsSize("FileName");
            AllowFitColumsSize("FolderName");
            AllowFitColumsSize("EarliestDate");

            //m_fastObjectListView.SetObjects(metadataLogList);
            //m_fastObjectListView.RefreshObjects(metadataLogList);
            m_fastObjectListView.Objects = metadataLogList;
        }

        private bool AllowFitColumsSize(string headerText)
        {
            var fitSizeColums = m_fastObjectListView.AllColumns.Find(x => x.AspectName == headerText);
            if (fitSizeColums != null)
            {
                fitSizeColums.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                return true;
            }
            return false;
        }

        private void m_ObjectListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            try
            {
                if (e.ItemIndex >= 0 && e.ItemIndex < metadataLogList.Count)
                {
                    var item = new OLVListItem("No");
                    foreach (ColumnHeaderCollection column in m_fastObjectListView.Columns)
                    {
                        var property = typeof(MetadataModifyLog).GetProperty(column.ToString());
                        if (property != null)
                        {
                            var value = property.GetValue(metadataLogList[e.ItemIndex]);
                            if (value is DateTime dateTimeValue && dateTimeValue == DateTime.MinValue)
                            {
                                // Display a null string for DateTime.MinValue values
                                item.SubItems.Add(new OLVListSubItem(column, value != null ? value.ToString() : "", " "));
                            }
                            else
                            {
                                item.SubItems.Add(new OLVListSubItem(column, value != null ? value.ToString() : "", value));
                            }
                        }
                    }
                    e.Item = item;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error m_ObjectListView_RetrieveVirtualItem[{e.ItemIndex}]: {e.Item.ImageKey} {ex.Message}");
            }
        }

        private void m_listMouseMove(object sender, MouseEventArgs e)
        {
            var hitTestInfo = m_fastObjectListView.HitTest(e.Location);
            if (hitTestInfo != null && hitTestInfo.Item != null && hitTestInfo.SubItem != null)
            {
                var subItemRect = hitTestInfo.SubItem.Bounds;
                var toolTipText = hitTestInfo.SubItem.Text;

                // Show the tooltip at the mouse cursor position
                m_toolTip.Show(toolTipText, m_fastObjectListView, e.Location.X, e.Location.Y);
            }
            else
            {
                // Hide the tooltip if the mouse is not over a subitem
                m_toolTip.Hide(m_fastObjectListView);
            }
        }
    }
}
