using Temple.Domain.Entities.DD;

namespace Temple.ViewModel.DD.Battle.BusinessLogic
{
    public static class BattleSceneFactory
    {
        public static Scene GetBattleScene(string battleSceneId)
        {
            return battleSceneId switch
            {
                "Dungeon 1, Room 1, Goblin" => GetSceneFirstBattle(),
                "Final Battle" => GetSceneFinalBattle(),
                _ => throw new ArgumentException($"Unknown battle scene ID: {battleSceneId}", nameof(battleSceneId))
            };
        }

        private static Scene GetSceneFirstBattle()
        {
            var knight = new CreatureType("Knight",
                maxHitPoints: 8, //20,
                armorClass: 3,
                thaco: 1, //12,
                initiativeModifier: 0,
                movement: 4,
                attacks: new List<Attack>
                {
                    new MeleeAttack("Longsword", 100),// 10),
                    new MeleeAttack("Longsword", 100),// 10),
                    new MeleeAttack("Longsword", 100),// 10),
                    new MeleeAttack("Longsword", 100),// 10)
                });

            var goblin = new CreatureType(
                name: "Goblin",
                maxHitPoints: 12,
                armorClass: 5,
                thaco: 20,
                initiativeModifier: 0,
                movement: 6,
                attacks: new List<Attack>
                {
                    new MeleeAttack("Short sword", 6)
                });

            var archer = new CreatureType(
                name: "Archer",
                maxHitPoints: 12,
                armorClass: 6,
                thaco: 14,
                initiativeModifier: 10,
                movement: 6,
                attacks: new List<Attack>
                {
                    new RangedAttack(
                        name: "Bow & Arrow",
                        maxDamage: 4,
                        range: 5)
                });

            var goblinArcher = new CreatureType(
                name: "Goblin Archer",
                maxHitPoints: 20,
                armorClass: 7,
                thaco: 13,
                initiativeModifier: 0,
                movement: 6,
                attacks: new List<Attack>
                {
                    new RangedAttack(
                        name: "Bow & Arrow",
                        maxDamage: 4,
                        range: 4)
                });

            var scene = new Scene("DummyScene", 8, 7);
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 4, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 1));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 6));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 4, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 6));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 6));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 1));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 1));
            scene.AddCreature(new Creature(knight, false) { IsAutomatic = false }, 5, 3);
            scene.AddCreature(new Creature(goblin, true) { IsAutomatic = true }, 1, 4);

            return scene;
        }

        private static Scene GetSceneFinalBattle()
        {
            var knight = new CreatureType("Knight",
                maxHitPoints: 8, //20,
                armorClass: 3,
                thaco: 1, //12,
                initiativeModifier: 0,
                movement: 4,
                attacks: new List<Attack>
                {
                    new MeleeAttack("Longsword", 100),// 10),
                    new MeleeAttack("Longsword", 100),// 10),
                    new MeleeAttack("Longsword", 100),// 10),
                    new MeleeAttack("Longsword", 100),// 10)
                });

            var goblin = new CreatureType(
                name: "Goblin",
                maxHitPoints: 12,
                armorClass: 5,
                thaco: 20,
                initiativeModifier: 0,
                movement: 6,
                attacks: new List<Attack>
                {
                    new MeleeAttack("Short sword", 6)
                });

            var archer = new CreatureType(
                name: "Archer",
                maxHitPoints: 12,
                armorClass: 6,
                thaco: 14,
                initiativeModifier: 10,
                movement: 6,
                attacks: new List<Attack>
                {
                    new RangedAttack(
                        name: "Bow & Arrow",
                        maxDamage: 4,
                        range: 5)
                });

            var goblinArcher = new CreatureType(
                name: "Goblin Archer",
                maxHitPoints: 20,
                armorClass: 7,
                thaco: 13,
                initiativeModifier: 0,
                movement: 6,
                attacks: new List<Attack>
                {
                    new RangedAttack(
                        name: "Bow & Arrow",
                        maxDamage: 4,
                        range: 4)
                });

            var scene = new Scene("DummyScene", 11, 12);
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 4, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 8, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 9, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 10, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 1));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 1));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 3));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 4));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 6));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 6));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 8));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 8));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 4, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 8, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 9, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 10, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 9));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 10));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 10));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 10));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 10));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 4, 10));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 10));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 8, 10));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 9, 10));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 10, 10));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 10));
            scene.AddCreature(new Creature(knight, false) { IsAutomatic = false }, 6, 9);
            scene.AddCreature(new Creature(goblinArcher, true) { IsAutomatic = true }, 4, 1);
            scene.AddCreature(new Creature(goblinArcher, true) { IsAutomatic = true }, 7, 1);

            return scene;
        }
    }
}
