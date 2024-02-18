using Godot;
using System;
using System.Collections.Generic;
using System.IO;

namespace Game
{
    [GlobalClass]
    public partial class GameData : Node
    {
        public static GameData Instance { get; private set; }

        private Dictionary<string, PackedScene> items;
        // private Dictionary<string, PackedScene> enemies;
        private Dictionary<string, PackedScene> projectiles;
        private Dictionary<string, AudioStream > soundEffects;
        private const string DIR_ITEMS_PATH = "res://Scenes/Items/";
        private const string DIR_PROJECTILES_PATH = "res://Scenes/Projectiles/";
        private const string DIR_SOUND_PATH = "res://Assets/Sounds/";


       // private const string DIR_ITEMS_PATH = "C:\\Users\\moses\\Documents\\RPGDEMO\\Scenes\\Items";
        // private const string DIR_ENEMIES_PATH = "res://Scenes/Models/Enemies/";
        //private const string DIR_PROJECTILES_PATH = "C:\\Users\\moses\\Documents\\RPGDEMO\\Scenes\\Projectiles";
        //private const string DIR_SOUND_PATH = "C:\\Users\\moses\\Documents\\RPGDEMO\\Assets\\Sounds";

        public override void _Ready()
        {
            // = ProjectSettings.GlobalizePath(DIR_ITEMS_PATH)
            items = GetDirContents(DIR_ITEMS_PATH, ".tscn");
            //enemies = GetDirContents(DIR_ENEMIES_PATH, ".tscn");
            projectiles = GetDirContents(DIR_PROJECTILES_PATH, ".tscn");
            soundEffects = GetAudioContents(DIR_SOUND_PATH, ".mp3");
            Merge(soundEffects, GetAudioContents(DIR_SOUND_PATH, ".wav"));
            Instance = this;
        }

        public Projectile FetchProjectile(string projName)
        {
            if (projectiles.ContainsKey(projName))
            {
                Projectile proj = (Projectile)projectiles[projName].Instantiate();
                return proj;
            }
            return null;
        }

        public Item FetchItem(string itemName)
        {
            if (items.ContainsKey(itemName))
            {
                Item item = (Item)items[itemName].Instantiate();
                return item;
            }
            return null;
        }

        // public PackedScene FetchEnemyScene(string enemyScene)
        // {
        //     if (enemies.ContainsKey(enemyScene))
        //     {
        //         return enemies[enemyScene];
        //     }
        //     return null;
        // }

        public AudioStream FetchSound(string sound)
        {
            if (soundEffects.ContainsKey(sound)) return soundEffects[sound];
            
            return null;
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            Instance.Free();
        }
        

        private Dictionary<string, AudioStream > GetAudioContents(string path, string filesSuffix)
        {
            var dir = DirAccess.Open(path);
            // Check if the directory exists
            if (dir != null)
            {
                // Initialize the dictionary to hold the file contents
                var fileContents = new Dictionary<string, AudioStream>();
                dir.ListDirBegin();
                // Get all files in the directory
                //string[] files = Directory.GetFileSystemEntries(path);
                // Iterate over each file in the directory
                
                        // Get the file name without the suffix
                var fileName = dir.GetNext();
                while (fileName != "")
                {
                    if (dir.CurrentIsDir())
                    {
                        //var new_dir = DirAccess.Open(path+fileName);
                        Merge(fileContents, GetAudioContents(path+fileName+"/", filesSuffix));
                    }
                    else
                    {
                        fileName = fileName.Replace(".remap", "");
                        fileName = fileName.Replace(".import", "");
                        
                        if (fileName.EndsWith(filesSuffix))
                        {
                            AudioStream audioStream = (AudioStream )ResourceLoader.Load(path+fileName);
                            fileName = fileName.Replace(filesSuffix, "");
                            if (fileContents.ContainsKey(fileName) == false)
                                {
                                    GD.Print("Loading file: " + fileName + filesSuffix);
                                    fileContents[fileName] = audioStream;
                                }
                        }
                    }
                    fileName = dir.GetNext();
                }
                //     else
                //     {
                //         // Check if it's a directory
                //         if (File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
                //         {
                //             Merge(fileContents, GetAudioContents(filePath, filesSuffix));
                //         }
                //     }
                // }

                // Return the dictionary containing file contents
                return fileContents;
            }
            else
            {
                // Directory does not exist, return null
                return null;
            }
        }

        private Dictionary<string, PackedScene> GetDirContents(string path, string filesSuffix)
        {
           var dir = DirAccess.Open(path);
            // Check if the directory exists
            if (dir != null)
            {
                // Initialize the dictionary to hold the file contents
                var fileContents = new Dictionary<string, PackedScene>();
                dir.ListDirBegin();
                // Get all files in the directory
                //string[] files = Directory.GetFileSystemEntries(path);
                // Iterate over each file in the directory
                
                        // Get the file name without the suffix
                var fileName = dir.GetNext();
                while (fileName != "")
                {
                    if (dir.CurrentIsDir())
                    {
                        //var new_dir = DirAccess.Open(path+fileName);
                        Merge(fileContents, GetDirContents(path+fileName+"/", filesSuffix));
                    }
                    else
                    {
                        fileName = fileName.Replace(".remap", "");
                        fileName = fileName.Replace(".import", "");
                        
                        if (fileName.EndsWith(filesSuffix))
                        {
                            PackedScene scene = (PackedScene )ResourceLoader.Load(path+fileName);
                            fileName = fileName.Replace(filesSuffix, "");
                            // Add the file name (without suffix) and its corresponding PackedScene to the dictionary
                            if (fileContents.ContainsKey(fileName) == false)
                                {
                                    GD.Print("Loading file: " + fileName + filesSuffix);
                                    fileContents[fileName] = scene;
                                }
                        }
                        
                    }
                    fileName = dir.GetNext();
                }
                //     else
                //     {
                //         // Check if it's a directory
                //         if (File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
                //         {
                //             Merge(fileContents, GetAudioContents(filePath, filesSuffix));
                //         }
                //     }
                // }

                // Return the dictionary containing file contents
                return fileContents;
            }
            else
            {
                // Directory does not exist, return null
                return null;
            }
        }

        private Dictionary<string, PackedScene> IterateOverDir(string[] dir, string path, string filesSuffix)
        {
            var ret = new Dictionary<string, PackedScene>();
            if (dir != null)
            {
                foreach (var file in dir)
                {
                    GD.Print(file);
                    // if (fileInfo.Attributes.HasFlag(FileAttributes.Directory))
                    // {
                    //     var newDir = new DirectoryInfo(Path.Combine(path, fileName));
                    //     Merge(ret,IterateOverDir(newDir, Path.Combine(path, fileName + "/"), filesSuffix));
                    // }
                    // else
                    // {
                    //     fileName = fileName.Replace(".remap", "");
                    //     fileName = fileName.Replace(".import", "");
                    //     if (fileName.EndsWith(filesSuffix))
                    //     {
                    //         GD.Print("Loading file: " + fileName);
                    //         ret[fileName.Substring(0, fileName.Length - filesSuffix.Length)] = (PackedScene)ResourceLoader.Load(Path.Combine(path, fileName));
                    //     }
                    // }
                }
            }
            else
            {
                GD.Print("An error occurred when trying to access the path.");
            }
            return ret;
        }

        private void Merge<TKey, TValue>(Dictionary<TKey, TValue> destination, Dictionary<TKey, TValue> source)
        {
            foreach (var kvp in source)
            {
                destination[kvp.Key] = kvp.Value;
            }
        }
    }
}