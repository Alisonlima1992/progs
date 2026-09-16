using System.Collections.Generic;
using System.Linq;

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
    }
}