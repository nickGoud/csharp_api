using Casestudy.DAL.DomainClasses;
using System.Text.Json;
using System;

namespace Casestudy.DAL
{
    public class DataUtility
    {
        private readonly AppDbContext _db;
        public DataUtility(AppDbContext context)
        {
            _db = context;
        }

        private async Task<bool> LoadBrands(dynamic jsonObjectArray)
        {
            bool loadedBrands = false;
            try
            {
                // clear out the old rows
                _db.Brands?.RemoveRange(_db.Brands);
                await _db.SaveChangesAsync();
                List<String> allBrands = new();
                foreach (JsonElement element in jsonObjectArray.EnumerateArray())
                {
                    if (element.TryGetProperty("BRAND", out JsonElement productJson))
                    {
                        allBrands.Add(productJson.GetString()!);
                    }
                }
                IEnumerable<String> brands = allBrands.Distinct<String>();
                foreach (string branname in brands)
                {
                    Brand bran = new();
                    bran.Name = branname;
                    await _db.Brands!.AddAsync(bran);
                    await _db.SaveChangesAsync();
                }
                loadedBrands = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - " + ex.Message);
            }
            return loadedBrands;
        }

        private async Task<bool> LoadProducts(dynamic jsonObjectArray)
        {
            bool loadedItems = false;
            try
            {
                List<Brand> brands = _db.Brands!.ToList();
                // clear outthe old
                _db.Products?.RemoveRange(_db.Products);
                await _db.SaveChangesAsync();
                foreach (JsonElement element in jsonObjectArray.EnumerateArray())
                {
                    Product item = new();

                    item.Id = element.GetProperty("ID").GetString();
                    item.ProductName = element.GetProperty("PRODUCTNAME").GetString();
                    item.CostPrice = Convert.ToDecimal(element.GetProperty("COSTPRICE").GetString());
                    item.MSRP = Convert.ToDecimal(element.GetProperty("MSRP").GetString());
                    item.QtyOnHand = Convert.ToInt32(element.GetProperty("QTYONHAND").GetString());
                    item.QtyOnBackOrder = Convert.ToInt32(element.GetProperty("QTYONBACKORDER").GetString());
                    item.Description = Convert.ToString(element.GetProperty("DESCRIPTION").GetString());

                    //graphic name
                    try { item.GraphicName = element.GetProperty("GRAPHICNAME").GetString(); }
                    catch { item.GraphicName = null; }


                    string? bran = element.GetProperty("BRAND").ToString();
                    // add the FK here
                    foreach (Brand brand in brands)
                    {
                        if (brand.Name == bran)
                        {
                            item.Brand = brand;
                            break;
                        }
                    }
                    await _db.Products!.AddAsync(item);
                    await _db.SaveChangesAsync();
                }
                loadedItems = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - " + ex.Message);
            }
            return loadedItems;
        }



        public async Task<bool> LoadNutritionInfoFromWebToDb(string stringJson)
        {
            bool brandsLoaded = false;
            bool productsLoaded = false;
            try
            {
                // an element that is typed as dynamic is assumed to support any operation
                dynamic? objectJson = JsonSerializer.Deserialize<Object>(stringJson);
                brandsLoaded = await LoadBrands(objectJson);
                productsLoaded = await LoadProducts(objectJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return brandsLoaded && productsLoaded;
        }
    }
}