using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using AddAdminDto = API.DTOs.AddAdminDto;
using AdminWithUserDto = API.DTOs.AdminWithUserDto;
using AdminWithClubDto = API.DTOs.AdminWithClubDto;
using ClubWithAdminDto = API.DTOs.ClubWithAdminDto;
using UpdateAdminDto = API.DTOs.UpdateAdminDto;
using DeleteAdminDto = API.DTOs.DeleteAdminDto;
using DataContext = API.Data.DataContext;

namespace API.Services{
    public class AdminService
    {
        private readonly DataContext _context;

        public AdminService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<AdminWithUserDto>> GetAdminsWithUsers(){
            return await (from user in _context.Users
                join admin in _context.Admins on user.Id equals admin.UserID
                select new AdminWithUserDto{
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    AdminId = admin.AdminId
                }).ToListAsync();
        }

        public async Task<List<AdminWithClubDto>> GetAdminsWithClubs(){
            return await (from user in _context.Users
                join admin in _context.Admins on user.Id equals admin.UserID
                join adminClub in _context.AdminClubs on admin.AdminId equals adminClub.AdminId
                join club in _context.Clubs on adminClub.ClubId equals club.ClubId
                select new AdminWithClubDto{
                    UserName = user.UserName,
                    Email = user.Email,
                    AdminId = admin.AdminId,
                    ClubName = club.ClubName
                }).ToListAsync();
        }

        public async Task<List<ClubWithAdminDto>> GetClubsWithAdmins(){
            return await (from club in _context.Clubs
                join adminClub in _context.AdminClubs on club.ClubId equals adminClub.ClubId
                join admin in _context.Admins on adminClub.AdminId equals admin.AdminId
                join user in _context.Users on admin.UserID equals user.Id
                select new ClubWithAdminDto{
                    ClubName = club.ClubName,
                    AdminName = user.UserName,
                    AdminEmail = user.Email
                }).ToListAsync();
        }

        public async Task<string> AddAdminToClubs(AddAdminDto dto)
        {
            if (dto == null || dto.ClubIds == null || !dto.ClubIds.Any())
                return ("Admin ID and Club IDs are required.");

            var admin = await _context.Admins.FindAsync(dto.AdminId);

            if(admin == null) return ("Admin not found");

            var existingAdminClubs = await _context.AdminClubs
                .Where(ac => ac.AdminId == dto.AdminId  && dto.ClubIds.Contains(ac.ClubId))
                .ToListAsync();
        
            var clubs = await _context.Clubs
                .Where(c => dto.ClubIds.Contains(c.ClubId))
                .ToListAsync();

            if(clubs.Count != dto.ClubIds.Count) return ("One or more clubs not found");

        

            var newAdminClubs = await Task.WhenAll(dto.ClubIds
            .Where(clubId => !existingAdminClubs.Any(ac => ac.ClubId == clubId))
            .Select(async clubId => {
                var club = await _context.Clubs.FindAsync(clubId);
                if(club == null) throw new Exception("Club not found");
                return new AdminClub{
                    AdminId = dto.AdminId,
                    ClubId = clubId,
                    Admin = admin,
                    Club = club
                };
            }));


            await _context.AdminClubs.AddRangeAsync(newAdminClubs);
            await _context.SaveChangesAsync();

            return ("Admin succesfully added to the clubs");
        }

        public async Task<String> UpdateAdminClubs(UpdateAdminDto dto)
        {
            var admin = await _context.Admins
                .Include(a => a.AdminClubs)
                .FirstOrDefaultAsync(a => a.AdminId == dto.AdminId);

            if(admin == null) return ("Admin not found");

            var adminClubsToRemove = admin.AdminClubs
                .Where(ac => !dto.NewClubIds.Contains(ac.ClubId))
                .ToList();

            _context.AdminClubs.RemoveRange(adminClubsToRemove);

            var existingClubIds = admin.AdminClubs.Select(ac => ac.ClubId).ToList();
        
        
            var adminClubsToAdd = await Task.WhenAll(dto.NewClubIds
                .Where(clubId => !existingClubIds.Contains(clubId))
                .Select(async clubId =>
                {
                    var club = await _context.Clubs.FindAsync(clubId);
                    if (club == null) throw new Exception("Club not found");

                    return new AdminClub
                    {
                        AdminId = dto.AdminId,
                        ClubId = clubId,
                        Admin = admin,
                        Club = club
                    };
                }));


            _context.AdminClubs.AddRange(adminClubsToAdd);
            await _context.SaveChangesAsync();
            return ("Admin clubs updated successfully");
        }

        public async Task<string> DeleteAdmin(DeleteAdminDto dto)
        {
            var adminClubs = await _context.AdminClubs
                .Where(ac => ac.AdminId == dto.AdminId)
                .ToListAsync();

            if (!adminClubs.Any()) return ("No assigned clubs found for this admin");

            _context.AdminClubs.RemoveRange(adminClubs);
            await _context.SaveChangesAsync();

            return ("Admin club relation deleted succesfully!");
        }

        public async Task<string> EnsureAdminExists(int userId){
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.UserID == userId);

            if(admin == null){
                var user = await _context.Users.FindAsync(userId);
                if(user == null) return ("User not found");
                var newAdmin = new Admin{
                    UserID = userId,
                    User = user
                };
                _context.Admins.Add(newAdmin);
                await _context.SaveChangesAsync();
                return ("Admin created successfully");
            }
            return ("Admin already exists");
        }

    }
}
