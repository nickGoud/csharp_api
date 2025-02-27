using Casestudy.DAL.DomainClasses;
using CaseStudyAPI.DAL.DomainClasses;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Casestudy.DAL.DAO
{
    public class BranchDAO
    {
        private AppDbContext _db;
        public BranchDAO(AppDbContext _ctx)
        {
            _db = _ctx;
        }
        public async Task<List<Branch>?> GetThreeClosestBranches(float? lat, float? lon)
        {
            List<Branch>? branchDetails = null;
            try
            {
                var latParam = new SqlParameter("@lat", lat);
                var lonParam = new SqlParameter("@lon", lon);// forgot to rename so it just says stores
                var query = _db.Branches?.FromSqlRaw("dbo.pGetThreeClosestStores @lat, @lon", latParam, lonParam);
                branchDetails = await query!.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return branchDetails;
        }
        public async Task<bool> LoadBranchesFromFile(string path)
        {
            bool addWorked = false;
            try
            {
                // clear out the old rows
                _db.Branches?.RemoveRange(_db.Branches);
                await _db.SaveChangesAsync();
                var csv = new List<string[]>();
                var csvFile = path + "\\exercisesBranchRaw.txt";
                var lines = await System.IO.File.ReadAllLinesAsync(csvFile);
                foreach (string line in lines)
                    csv.Add(line.Split(',')); // populate branch with csv
                foreach (string[] rawdata in csv)
                {
                    Branch aBranch = new();
                    aBranch.Longitude = Convert.ToDouble(rawdata[0]);
                    aBranch.Latitude = Convert.ToDouble(rawdata[1]);
                    aBranch.Street = rawdata[2];
                    aBranch.City = rawdata[3];
                    aBranch.Region = rawdata[4];
                    await _db.Branches!.AddAsync(aBranch);
                    await _db.SaveChangesAsync();
                }
                addWorked = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return addWorked;
        }
    }
}
