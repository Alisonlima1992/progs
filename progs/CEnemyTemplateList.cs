using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace EnemyEditor
{
    public class CEnemyTemplateList
    {
        private List<CEnemyTemplate> enemies;

        public CEnemyTemplateList()
        {
            enemies = new List<CEnemyTemplate>();
        }

        public void AddEnemy(string name, string iconName, int baseLife,
            double lifeModifier, int baseGold,
            double goldModifier, double spawnChance)
        {
            enemies.Add(new CEnemyTemplate(
                name, iconName, baseLife,
                lifeModifier, baseGold, goldModifier, spawnChance));
        }

        public CEnemyTemplate GetEnemyByName(string name)
        {
            return enemies.FirstOrDefault(e => e.Name == name);
        }

        public CEnemyTemplate GetEnemyByIndex(int id)
        {
            if (id < 0 || id >= enemies.Count)
                return null;

            return enemies[id];
        }

        public void DeleteEnemyByName(string name)
        {
            var enemy = GetEnemyByName(name);
            if (enemy != null)
                enemies.Remove(enemy);
        }

        public void DeleteEnemyByIndex(int id)
        {
            if (id < 0 || id >= enemies.Count)
                return;

            enemies.RemoveAt(id);
        }

        public List<string> GetListOfEnemyNames()
        {
            return enemies.Select(e => e.Name).ToList();
        }

        public int Count => enemies.Count;

        public void SaveToJson(string path)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string jsonString = JsonSerializer.Serialize(enemies, options);
            File.WriteAllText(path, jsonString);
        }

        public void LoadFromJson(string path)
        {
            string jsonFromFile = File.ReadAllText(path);

            enemies.Clear();

            using JsonDocument doc = JsonDocument.Parse(jsonFromFile);

            foreach (JsonElement element in doc.RootElement.EnumerateArray())
            {
                string name = element.GetProperty("Name").GetString();
                string iconName = element.GetProperty("IconName").GetString();
                int baseLife = element.GetProperty("BaseLife").GetInt32();
                double lifeModifier = element.GetProperty("LifeModifier").GetDouble();
                int baseGold = element.GetProperty("BaseGold").GetInt32();
                double goldModifier = element.GetProperty("GoldModifier").GetDouble();
                double spawnChance = element.GetProperty("SpawnChance").GetDouble();

                enemies.Add(new CEnemyTemplate(
                    name, iconName, baseLife,
                    lifeModifier, baseGold, goldModifier, spawnChance));
            }
        }

        public List<CEnemyTemplate> GetEnemies()
        {
            return new List<CEnemyTemplate>(enemies);
        }
    }
}