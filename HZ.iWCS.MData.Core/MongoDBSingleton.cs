using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HZ.iWCS.MData.Core
{
    public class MongoDBSingleton
    {
        private static readonly MongoDBSingleton instance = new MongoDBSingleton();

        /// <summary>
        /// 获得MongoDB的数据库实体
        /// </summary>
        private IMongoDatabase mDatabase { get; set; }

        /// <summary>
        /// 连接状态
        /// </summary>
        public bool Connectioned { get; set; } = false;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connection">MongoDB连接字符串</param>
        /// <param name="database">数据库名称</param>
        public void InitMongoDB(string connection, string database)
        {
            if (mDatabase == null)
            {
                mDatabase = MongodbClient.MongodbDatabase(connection, database);

                Connectioned = MongodbClient.Connectioned();
            }
        }

        public void InitMongoDB(string ipAddress, int port, string databaseName,string userName,string passaWord)
        {
            if (mDatabase == null)
            {
                try
                {
                    mDatabase = MongodbClient.MongodbDatabase(ipAddress, port, databaseName, userName, passaWord);

                    Connectioned = true;

                }
                catch { }
            }
        }

        /// <summary>
        /// 获取单实例
        /// </summary>
        public static MongoDBSingleton Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// ObjectId的键
        /// </summary>
        private readonly string OBJECTID_KEY = "_id";


        #region 获取所有对象集合
        public List<string> GetCollectionNames()
        {
            return mDatabase.ListCollectionNames().ToList();
        }
        #endregion

        #region +Add 添加一条数据
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="t">添加的实体</param>
        /// <param name="collectionName">表名称(不填则默认为实体类名称)</param>
        /// <returns></returns>
        public bool Add<T>(T t, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                var client = mDatabase.GetCollection<T>(collectionName);
                client.InsertOne(t);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region +AddAsync 异步添加一条数据
        /// <summary>
        /// 异步添加一条数据
        /// </summary>
        /// <param name="t">添加的实体</param>
        /// <param name="host">mongodb连接信息</param>
        /// <returns></returns>
        public async Task<bool> AddAsync<T>(T t, string collectionName)
        {
            try
            {
                await mDatabase.GetCollection<T>(collectionName).InsertOneAsync(t);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region +InsertMany 批量插入
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="t">实体集合</param>
        /// <returns></returns>
        public bool InsertMany<T>(List<T> t, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                mDatabase.GetCollection<T>(collectionName).InsertMany(t);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region +InsertManyAsync 异步批量插入
        /// <summary>
        /// 异步批量插入
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="t">实体集合</param>
        /// <returns></returns>
        public async Task<bool> InsertManyAsync<T>(List<T> t, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                await mDatabase.GetCollection<T>(collectionName).InsertManyAsync(t);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region +Update 修改一条数据
        /// <summary>
        /// 修改一条数据(根据_id)
        /// </summary>
        /// <param name="t">添加的实体</param>
        /// <param name="host">mongodb连接信息</param>
        /// <returns></returns>
        public UpdateResult Update<T>(T t, string _objectID, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                //修改条件
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(_objectID));
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (item.Name.ToLower() == "id") continue;
                    list.Add(Builders<T>.Update.Set(item.Name, item.GetValue(t)));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return mDatabase.GetCollection<T>(collectionName).UpdateOne(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region +UpdateFilter 修改一条数据
        /// <summary>
        /// 修改一条数据(根据Filter条件)
        /// </summary>
        /// <param name="t">添加的实体</param>
        /// <param name="filter">更新条件</param>
        /// <param name="collectionName">表集合</param>
        /// <returns></returns>
        public UpdateResult UpdateFilter<T>(T t, FilterDefinition<T> filter, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (item.Name.ToLower() == "id") continue;
                    list.Add(Builders<T>.Update.Set(item.Name, item.GetValue(t)));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return mDatabase.GetCollection<T>(collectionName).UpdateOne(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region +UpdateAsync 异步修改一条数据
        /// <summary>
        /// 异步修改一条数据
        /// </summary>
        /// <param name="t">添加的实体</param>
        /// <param name="host">mongodb连接信息</param>
        /// <returns></returns>
        public async Task<UpdateResult> UpdateAsync<T>(T t, string id, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                //var client = MongodbClient<T>.MongodbInfoClient(host, collectionName);
                //修改条件
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (item.Name.ToLower() == "id") continue;
                    list.Add(Builders<T>.Update.Set(item.Name, item.GetValue(t)));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return await mDatabase.GetCollection<T>(collectionName).UpdateOneAsync(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region +UpdateManay 批量修改数据
        /// <summary>
        /// 批量修改数据
        /// </summary>
        /// <param name="updatefilter">要修改的字段</param>
        /// <param name="filter">修改条件</param>
        /// <param name="collectionName">表名称</param>
        /// <returns></returns>
        public UpdateResult UpdateManay<T>(UpdateDefinition<T> updatefilter, FilterDefinition<T> filter, string collectionName)
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                return mDatabase.GetCollection<T>(collectionName).UpdateMany(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region +UpdateManayAsync 异步批量修改数据
        /// <summary>
        /// 异步批量修改数据
        /// </summary>
        /// <param name="updatefilter">要修改的字段</param>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">修改条件</param>
        /// <returns></returns>
        public async Task<UpdateResult> UpdateManayAsync<T>(UpdateDefinition<T> updatefilter, FilterDefinition<T> filter, string collectionName)
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                return await mDatabase.GetCollection<T>(collectionName).UpdateManyAsync(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region +BulkWriteV2 批量写入数据
        /// <summary>
        /// 批量写入数据V2。
        /// 时间：2026-06-10
        /// 优化内容：支持把多条不同条件、不同内容的写入操作合并成一次MongoDB BulkWrite提交。
        /// 适用场景：例如设备最新值按deviceNo分别Upsert，每台设备的数据不同，不能使用UpdateManay统一更新成同一份数据。
        /// </summary>
        /// <param name="writeModels">批量写入模型集合，支持UpdateOneModel、InsertOneModel、DeleteOneModel等</param>
        /// <param name="collectionName">表名称(不填则默认为实体类名称)</param>
        /// <param name="isOrdered">是否按顺序执行；高频采集建议false，单条失败时减少对其他设备写入的影响</param>
        /// <returns>批量写入结果；无数据时返回null</returns>
        public BulkWriteResult<T> BulkWriteV2<T>(List<WriteModel<T>> writeModels, string collectionName = "", bool isOrdered = false)
        {
            try
            {
                if (writeModels == null || writeModels.Count == 0)
                {
                    return null;
                }

                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }

                BulkWriteOptions options = new BulkWriteOptions()
                {
                    IsOrdered = isOrdered
                };
                return mDatabase.GetCollection<T>(collectionName).BulkWrite(writeModels, options);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region +CreateAscendingIndexV2 创建升序索引
        /// <summary>
        /// 创建升序索引V2。
        /// 时间：2026-06-10
        /// 优化内容：为高频查询/Upsert字段创建索引，减少BulkWrite按条件匹配时的集合扫描。
        /// 注意：MongoDB重复创建同名同配置索引是幂等操作；如果历史数据可能重复，不建议直接创建唯一索引。
        /// </summary>
        /// <param name="fieldName">索引字段名称，例如deviceNo</param>
        /// <param name="collectionName">表名称(不填则默认为实体类名称)</param>
        /// <param name="indexName">索引名称，不填则自动生成</param>
        /// <param name="isUnique">是否唯一索引；默认false，避免历史重复数据导致启动失败</param>
        /// <returns>索引名称；参数无效时返回空字符串</returns>
        public string CreateAscendingIndexV2<T>(string fieldName, string collectionName = "", string indexName = "", bool isUnique = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fieldName))
                {
                    return "";
                }

                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }

                if (string.IsNullOrWhiteSpace(indexName))
                {
                    indexName = "idx_" + collectionName + "_" + fieldName;
                }

                CreateIndexOptions options = new CreateIndexOptions()
                {
                    Name = indexName,
                    Unique = isUnique,
                    Background = true
                };
                CreateIndexModel<T> indexModel = new CreateIndexModel<T>(Builders<T>.IndexKeys.Ascending(fieldName), options);
                return mDatabase.GetCollection<T>(collectionName).Indexes.CreateOne(indexModel);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region +BulkWriteAsyncV2 异步批量写入数据
        /// <summary>
        /// 异步批量写入数据V2。
        /// 时间：2026-06-10
        /// 优化内容：用于异步接口或后台任务，将多条不同内容的Mongo写入合并成一次网络往返。
        /// </summary>
        /// <param name="writeModels">批量写入模型集合</param>
        /// <param name="collectionName">表名称(不填则默认为实体类名称)</param>
        /// <param name="isOrdered">是否按顺序执行；默认false提升批量写入吞吐</param>
        /// <returns>批量写入结果；无数据时返回null</returns>
        public async Task<BulkWriteResult<T>> BulkWriteAsyncV2<T>(List<WriteModel<T>> writeModels, string collectionName = "", bool isOrdered = false)
        {
            try
            {
                if (writeModels == null || writeModels.Count == 0)
                {
                    return null;
                }

                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }

                BulkWriteOptions options = new BulkWriteOptions()
                {
                    IsOrdered = isOrdered
                };
                return await mDatabase.GetCollection<T>(collectionName).BulkWriteAsync(writeModels, options);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Delete 删除一条数据
        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="id">objectId</param>
        /// <param name="collectionName">表名称</param>
        /// <returns></returns>
        public DeleteResult Delete<T>(string id, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                //var client = MongodbClient<T>.MongodbInfoClient(host, collectionName);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                return mDatabase.GetCollection<T>(collectionName).DeleteOne(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region Delete 删除所有数据
        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="collectionName">表名称</param>
        /// <returns></returns>
        public DeleteResult DeleteAll<T>(string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                FilterDefinition<T> filter = Builders<T>.Filter.Empty;
                return mDatabase.GetCollection<T>(collectionName).DeleteMany(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region DeleteFilter 删除一条数据
        /// <summary>
        /// 根据条件删除一条数据
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <param name="collectionName">表名称</param>
        /// <returns></returns>
        public DeleteResult DeleteFilter<T>(FilterDefinition<T> filter, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                //var client = MongodbClient<T>.MongodbInfoClient(host, collectionName);
                return mDatabase.GetCollection<T>(collectionName).DeleteOne(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region DeleteAsync 异步删除一条数据
        /// <summary>
        /// 异步删除一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="id">objectId</param>
        /// <returns></returns>
        public async Task<DeleteResult> DeleteAsync<T>(string id, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                //var client = MongodbClient<T>.MongodbInfoClient(host, collectionName);
                //修改条件
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                return await mDatabase.GetCollection<T>(collectionName).DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region DeleteMany 删除多条数据
        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">删除的条件</param>
        /// <returns></returns>
        public DeleteResult DeleteMany<T>(FilterDefinition<T> filter, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                //var client = MongodbClient<T>.MongodbInfoClient(host, collectionName);
                return mDatabase.GetCollection<T>(collectionName).DeleteMany(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region DeleteManyAsync 异步删除多条数据
        /// <summary>
        /// 异步删除多条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">删除的条件</param>
        /// <returns></returns>
        public async Task<DeleteResult> DeleteManyAsync<T>(FilterDefinition<T> filter, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                // var client = MongodbClient<T>.MongodbInfoClient(host, collectionName);
                return await mDatabase.GetCollection<T>(collectionName).DeleteManyAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Count 根据条件获取总数
        /// <summary>
        /// 根据条件获取总数
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public long Count<T>(FilterDefinition<T> filter, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                //var client = MongodbClient<T>.MongodbInfoClient(host, collectionName);Count(filter)
                return mDatabase.GetCollection<T>(collectionName).CountDocuments(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region CountAsync 异步根据条件获取总数
        /// <summary>
        /// 异步根据条件获取总数
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public async Task<long> CountAsync<T>(FilterDefinition<T> filter, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                //var client = MongodbClient<T>.MongodbInfoClient(host, collectionName);Count(filter)
                return await mDatabase.GetCollection<T>(collectionName).CountDocumentsAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindOne 根据id查询一条数据
        /// <summary>
        /// 根据id查询一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="id">objectid</param>
        /// <param name="field">要查询的字段，不写时查询全部</param>
        /// <returns></returns>
        public T FindOne<T>(string id, string collectionName = "", string[] field = null)
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                //var client = MongodbClient<T>.MongodbInfoClient(host, collectionName);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    return mDatabase.GetCollection<T>(collectionName).Find(filter).FirstOrDefault<T>();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList.Clear();
                return mDatabase.GetCollection<T>(collectionName).Find(filter).Project<T>(projection).FirstOrDefault<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindOne 根据条件查询一条数据
        /// <summary>
        /// 根据id查询一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">查询条件</param>
        /// <param name="field">要查询的字段，不写时查询全部</param>
        /// <returns></returns>
        public T FindOneFilter<T>(FilterDefinition<T> filter, string collectionName = "", string[] field = null)
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                //var client = MongodbClient<T>.MongodbInfoClient(host, collectionName);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    return mDatabase.GetCollection<T>(collectionName).Find(filter).FirstOrDefault<T>();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList.Clear();
                return mDatabase.GetCollection<T>(collectionName).Find(filter).Project<T>(projection).FirstOrDefault<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindOneAsync 异步根据id查询一条数据
        /// <summary>
        /// 异步根据id查询一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="id">objectid</param>
        /// <returns></returns>
        public async Task<T> FindOneAsync<T>(string id, string collectionName = "", string[] field = null)
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                //var client = MongodbClient<T>.MongodbInfoClient(host, collectionName);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    return await mDatabase.GetCollection<T>(collectionName).Find(filter).FirstOrDefaultAsync();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList.Clear();
                return await mDatabase.GetCollection<T>(collectionName).Find(filter).Project<T>(projection).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindList 查询集合
        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">查询条件</param>
        /// <param name="field">要查询的字段,不写时查询全部</param>
        /// <param name="sort">要排序的字段</param>
        /// <returns></returns>
        public List<T> FindList<T>(FilterDefinition<T> filter, string[] field = null, SortDefinition<T> sort = null, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                //var client = MongodbClient<T>.MongodbInfoClient(host, collectionName);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (sort == null) return mDatabase.GetCollection<T>(collectionName).Find(filter).ToList();
                    //进行排序
                    return mDatabase.GetCollection<T>(collectionName).Find(filter).Sort(sort).ToList();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList.Clear();
                if (sort == null) return mDatabase.GetCollection<T>(collectionName).Find(filter).Project<T>(projection).ToList();
                //排序查询
                return mDatabase.GetCollection<T>(collectionName).Find(filter).Sort(sort).Project<T>(projection).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindList 查询前N条集合
        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">查询条件</param>
        /// <param name="topNum">条数</param>
        /// <param name="field">要查询的字段,不写时查询全部</param>
        /// <param name="sort">要排序的字段</param>
        /// <returns></returns>
        public List<T> FindListLimit<T>(FilterDefinition<T> filter, int topNum, string[] field = null, SortDefinition<T> sort = null, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                //var client = MongodbClient<T>.MongodbInfoClient(host, collectionName);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (sort == null) return mDatabase.GetCollection<T>(collectionName).Find(filter).Limit(topNum).ToList();
                    //进行排序
                    return mDatabase.GetCollection<T>(collectionName).Find(filter).Sort(sort).Limit(topNum).ToList();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList.Clear();
                if (sort == null) return mDatabase.GetCollection<T>(collectionName).Find(filter).Limit(topNum).Project<T>(projection).ToList();
                //排序查询
                return mDatabase.GetCollection<T>(collectionName).Find(filter).Sort(sort).Limit(topNum).Project<T>(projection).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindListAsync 异步查询集合
        /// <summary>
        /// 异步查询集合
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">查询条件</param>
        /// <param name="field">要查询的字段,不写时查询全部</param>
        /// <param name="sort">要排序的字段</param>
        /// <returns></returns>
        public async Task<List<T>> FindListAsync<T>(FilterDefinition<T> filter, string[] field = null, SortDefinition<T> sort = null, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                //var client = MongodbClient<T>.MongodbInfoClient(host, collectionName);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (sort == null) return await mDatabase.GetCollection<T>(collectionName).Find(filter).ToListAsync();
                    return await mDatabase.GetCollection<T>(collectionName).Find(filter).Sort(sort).ToListAsync();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList.Clear();
                if (sort == null) return await mDatabase.GetCollection<T>(collectionName).Find(filter).Project<T>(projection).ToListAsync();
                //排序查询
                return await mDatabase.GetCollection<T>(collectionName).Find(filter).Sort(sort).Project<T>(projection).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindListByPage 分页查询集合
        /*
        /// <summary>
        /// 分页查询集合
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">查询条件</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="count">总条数</param>
        /// <param name="field">要查询的字段,不写时查询全部</param>
        /// <param name="sort">要排序的字段</param>
        /// <returns></returns>
        public List<T> FindListByPage<T>(FilterDefinition<T> filter, int pageIndex, int pageSize, out long count, string[] field = null, SortDefinition<T> sort = null, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                //var client = MongodbClient<T>.MongodbInfoClient(host, collectionName);
                count = mDatabase.GetCollection<T>(collectionName).CountDocuments(filter);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (sort == null) return  mDatabase.GetCollection<T>(collectionName).Find(filter).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();
                    //进行排序
                    return  mDatabase.GetCollection<T>(collectionName).Find(filter).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();

                //不排序
                if (sort == null) return  mDatabase.GetCollection<T>(collectionName).Find(filter).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();

                //排序查询
                return  mDatabase.GetCollection<T>(collectionName).Find(filter).Sort(sort).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindListByPageAsync 异步分页查询集合
        /// <summary>
        /// 异步分页查询集合
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">查询条件</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="field">要查询的字段,不写时查询全部</param>
        /// <param name="sort">要排序的字段</param>
        /// <returns></returns>
        public async Task<List<T>> FindListByPageAsync<T>(FilterDefinition<T> filter, int pageIndex, int pageSize, string[] field = null, SortDefinition<T> sort = null, string collectionName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = typeof(T).Name;
                }
                //var client = MongodbClient<T>.MongodbInfoClient(host, collectionName);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (sort == null) return await mDatabase.GetCollection<T>(collectionName).Find(filter).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();
                    //进行排序
                    return await mDatabase.GetCollection<T>(collectionName).Find(filter).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();

                //不排序
                if (sort == null) return await mDatabase.GetCollection<T>(collectionName).Find(filter).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();

                //排序查询
                return await mDatabase.GetCollection<T>(collectionName).Find(filter).Sort(sort).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        */
        #endregion
    }
}
