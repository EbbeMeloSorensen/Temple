using Temple.Application.State;
using Temple.Domain.Entities.DD.Battle;

namespace Temple.ViewModel.DD.Battle.BusinessLogic
{
    public static class BattleSceneFactory
    {
        public static Scene SetupBattleScene(
            List<Creature> party,
            string battleSceneId,
            string? entranceId)
        {
            return battleSceneId switch
            {
                "Dungeon 1, Room A, Goblin" => GetSceneA(party),
                "Dungeon 1, Room B, Goblin" => GetSceneB(party, entranceId),
                "Final Battle" => GetSceneF(party, entranceId),
                _ => throw new ArgumentException($"Unknown battle scene ID: {battleSceneId}", nameof(battleSceneId))
            };
        }

        private static Scene GetSceneA(
            List<Creature> party)
        {
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

            var scene = new Scene("DummyScene", 8, 6);

            // Obstacles
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 4, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 0));
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
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 6));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 1));

            // Enemies
            scene.AddCreature(new Creature(goblin, true) { IsAutomatic = true }, 1, 4);

            // Party
            var adventurerPositions = new List<Tuple<int, int>>
            {
                new (4, 3),
                new (4, 4),
                new (5, 3),
                new (5, 4)
           };

            adventurerPositions
                .Zip(party.Where(_ => _.HitPoints > 0), (position, adventurer) => new { position, adventurer})
                .ToList()
                .ForEach(_ =>
                {
                    scene.AddCreature(_.adventurer, _.position.Item1, _.position.Item2);
                });

            return scene;
        }

        private static Scene GetSceneB(
            List<Creature> party,
            string? entranceId)
        {
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

            var scene = new Scene("DummyScene", 8, 6);

            // Obstacles
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 4, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 0));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 1));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 6));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 4, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 7));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 6));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 5));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 1));

            // Enemies
            scene.AddCreature(new Creature(goblin, true) { IsAutomatic = true }, 3, 4);
            scene.AddCreature(new Creature(goblin, true) { IsAutomatic = true }, 4, 4);

            // Party
            var adventurerPositions = entranceId switch
            {
                "West" => new List<Tuple<int, int>>
                {
                    new (1, 4),
                    new (1, 3),
                    new (0, 4),
                    new (0, 3)
                },
                "East" => new List<Tuple<int, int>>
                {
                    new (4, 3),
                    new (4, 4),
                    new (5, 3),
                    new (5, 4)
                },
                _ => throw new InvalidOperationException()
            };

            adventurerPositions
                .Zip(party.Where(_ => _.HitPoints > 0), (position, adventurer) => new { position, adventurer })
                .ToList()
                .ForEach(_ =>
                {
                    scene.AddCreature(_.adventurer, _.position.Item1, _.position.Item2);
                });

            return scene;
        }

        private static Scene GetSceneF(
            IEnumerable<Creature> party,
            string? entranceId)
        {
            if (entranceId == null)
            {
                throw new InvalidOperationException("Entrance Id needed");
            }

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

            var scene = new Scene("DummyScene", 10, 12);
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
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 6));
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

            // Enemies
            scene.AddCreature(new Creature(goblinArcher, true) { IsAutomatic = true }, 4, 1);
            scene.AddCreature(new Creature(goblinArcher, true) { IsAutomatic = true }, 7, 1);

            // Party
            var adventurerPositions = entranceId switch
            {
                "South" => new List<Tuple<int, int>>
                {
                    new (6, 8),
                    new (5, 8),
                    new (6, 9),
                    new (5, 9)
                },
                "East" => new List<Tuple<int, int>>
                {
                    new (10, 5),
                    new (10, 6),
                    new (11, 5),
                    new (11, 6)
                },
                _ => throw new InvalidOperationException()
            };

            adventurerPositions
                .Zip(party.Where(_ => _.HitPoints > 0), (position, adventurer) => new { position, adventurer })
                .ToList()
                .ForEach(_ =>
                {
                    scene.AddCreature(_.adventurer, _.position.Item1, _.position.Item2);
                });

            return scene;
        }
    }
}
