using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MapAround.Mapping;

namespace MapAroundPathFinding
{
    public partial class LayerSettingsForm : Form
    {
        private Map _mapAroundMap;

        public event Action<LayerBase> OnLayerSettingsChanged;

        class LayerCheckItem
        {
            public LayerBase Layer { get; set; }

            public bool IsChecked
            {
                get => Layer.Visible;
                set => SetChecked(value);
            }

            public void SetChecked(bool val)
            {
                Layer.Visible = val;
                Debug.WriteLine($"Layer {Layer.GetType()}({Layer.Alias}) visibility changed to {Layer.Visible}");
            }

            public LayerCheckItem(LayerBase layerBase)
            {
                Layer = layerBase;
                IsChecked = layerBase.Visible;
            }
            public override string ToString()
            {
                return Layer.Alias;
            }
        }

        public LayerSettingsForm(Map mapAroundMap)
        {
            InitializeComponent();

            _mapAroundMap = mapAroundMap;
        }

        private void CreateLayerList()
        {
            for (var i = 0; i < _mapAroundMap.Layers.Count; i++)
            {
                var layer = _mapAroundMap.Layers[i];
                LayerCheckItem item = new LayerCheckItem(layer);
                LayersCheckedList.Items.Add(item);
                LayersCheckedList.SetItemChecked(i, item.IsChecked);
            }
        }

        private void LayerSettingsForm_Load(object sender, EventArgs e)
        {
            CreateLayerList();
        }

        private void LayersCheckedList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int index = e.Index;
            LayerCheckItem item = (LayerCheckItem)LayersCheckedList.Items[index];
            item.IsChecked = e.NewValue == CheckState.Checked;
            OnLayerSettingsChanged?.Invoke(item.Layer);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
