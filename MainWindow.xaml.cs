using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace FileViewerExercise
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var separators = new char[] // Separators \ /
             {
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar
             };
            List<string> myPaths;

            using (StreamReader readData = File.OpenText("data.json")) // Read data from json file
            {
                string json = readData.ReadToEnd();
                myPaths = JsonConvert.DeserializeObject<List<string>>(json); // Deserialize json to list<string> myPaths
            }

            LoadTreeView(myPaths, separators); // Load tree with data
        }

        private void LoadTreeView(IEnumerable<string> paths, char[] pathSeparator)
        {
            List<FileViewerItem> sourceCollection = new List<FileViewerItem>();
            foreach (string path in paths)
            {
                string[] Items = path.Split(pathSeparator); // split the paths
                FileViewerItem root = sourceCollection.FirstOrDefault(x => x.Name.Equals(Items[0]) && x.Level.Equals(1)); // Grab first item where Path depth is set to 1
                if (root == null)
                {
                    root = new FileViewerItem() // if no root exists, create a root with subitems
                    {
                        Level = 1,
                        Name = Items[0],
                        SubItems = new List<FileViewerItem>()
                    };
                    sourceCollection.Add(root);
                }

                if (Items.Length > 1) // if there still exists items after root is set, dig deeper.
                {
                    FileViewerItem parentItem = root;
                    int level = 2; // start on next depth.
                    for (int i = 1; i < Items.Length; ++i)
                    {
                        FileViewerItem subItem = parentItem.SubItems.FirstOrDefault(x => x.Name.Equals(Items[i]) && x.Level.Equals(level));
                        if (subItem == null)
                        {
                            subItem = new FileViewerItem() // if no child exists on level 2 but item does create childRoot and add items.
                            {
                                Name = Items[i],
                                Level = level,
                                SubItems = new List<FileViewerItem>()
                            };
                            parentItem.SubItems.Add(subItem);
                        }

                        parentItem = subItem;
                        level++; // Increment to go deeper in paths
                    }
                }
            }

            fileViewer.ItemsSource = sourceCollection;
        }
    }

    public class FileViewerItem
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public List<FileViewerItem> SubItems { get; set; }
    }
}
