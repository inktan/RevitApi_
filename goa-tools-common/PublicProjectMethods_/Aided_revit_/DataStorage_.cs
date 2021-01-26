﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;


namespace PublicProjectMethods_.Aided_revit_
{
    class DataStorage_
    {
        // 【】默认使用的 goa common 数据管理模式

        // 【0】初始化

        // 【1】判断guid-schema是否存在-对应 ElementListOfId                  -AddArrayField("ElementList", typeof(string))
        // 【1】判断guid-schema是否存在-对应 AutoGeneratedElementMgrSchema    -AddMapField("ElementListOfId", typeof(string), typeof(Entity))

        // 【2】判断schema-AutoGeneratedElementMgrSchema-entity-DataStorage是否存在，不存在则创建

        // 【3】实例化 AutoGeneratedElementMgr，找到已经创建的AutoGeneratedElementMgrSchema-entity-DataStorage

        // 【4】使用entity中的属性名及其类型Get<IDictionary<string, Entity>>("ElementListOfId")，从dataStorage中的一级entity，提取IDictionary<string, Entity>

        // 【5】从IDictionary<string, Entity>中提取二级new Entity(SchemaTypes.ElementListOfId)，如果不存在，则创建

        // 【6】使用二级entity中的属性名及其类型Get<IList<string>>("ElementList")，从entity，中提取IList<string>

        // 【7】Set二级Entity

        // 【8】Set一级DictonaryEntity


    }
}