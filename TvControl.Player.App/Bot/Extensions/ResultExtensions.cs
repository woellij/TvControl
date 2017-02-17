using System;
using System.Reflection;

using Syn.Bot.Oscova;
using Syn.Bot.Oscova.Entities;

using TvControl.Player.App.Bot.Entities;

namespace TvControl.Player.App.Bot.Extensions
{
    public static class ResultExtensions
    {

        public static Direction GetDirectionEnum(this Result result, Tuple<string, Type>[] entityTypeTuples)
        {
            foreach (Tuple<string, Type> entityTypeTuple in entityTypeTuples) {
                var entity = result.Entities.OfType<Entity>(entityTypeTuple.Item1);
                if (entity == null) {
                    continue;

                    
                }

                var mi = entity.GetType().GetMethod("ValueAs", BindingFlags.Instance | BindingFlags.Public);
                mi = mi.MakeGenericMethod(entityTypeTuple.Item2);
                var value = mi.Invoke(entity, new object[0]);
                return (Direction)(int) value;
            }
            return Direction.NONE
   ;     }
        

    }
}