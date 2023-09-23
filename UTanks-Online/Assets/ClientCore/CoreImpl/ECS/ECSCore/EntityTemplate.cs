using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.Core.Logging;

namespace UTanksClient.ECS.ECSCore
{
    [TypeUid(210198972242373120)]
    public abstract class EntityTemplate
    {

        //public EntityTemplate(EntityTemplate Template, List<string> ConfigPath)
        //{
        //    this.Template = Template;
        //    this.ConfigPath = ConfigPath;
        //}

        public abstract void InitializeConfigsPath();

        public ECSEntity ClientEntityExtend(ECSEntity entity)
        {
            entity.TemplateAccessorId.Add(this.GetId());
            return ClientEntityExtendImpl(entity);
        }

        protected abstract ECSEntity ClientEntityExtendImpl(ECSEntity entity);

        public override string ToString()
        {
            return $"[{Template.GetType().Name}, \"{ConfigPath}\"]";
        }

        public long GetId()
        {
            if (Id == 0)
                try
                {
                    if (TemplateAccessorType == null)
                    {
                        TemplateAccessorType = GetType();
                    }
                    if (ReflectionId == 0)
                        ReflectionId = TemplateAccessorType.GetCustomAttribute<TypeUidAttribute>().Id;
                    return ReflectionId;
                }
                catch
                {
                    ULogger.Error(this.GetType().ToString() + "Could not find Id field");
                    return 0;
                }
            else
                return Id;
        }

        public EntityTemplate Template { get; set; }
        public List<string> ConfigPath { get; set; } = new List<string>();
        public List<string> ConfigLibName { get; set; }
        public Type TemplateAccessorType;
        protected long ReflectionId = 0;
        public static long Id { get; set; }
    }
}
