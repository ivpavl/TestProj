using TestTask.Models;

namespace TestTask.Data.Services;
public class EstateService : IEstateService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public EstateService(AppDbContext context, IWebHostEnvironment environment)
    {
        _environment = environment;
        _context = context;
    }


    public List<EstateModel> GetEstatesListByUserId(int userId)
    {
        List<EstateModel> list = _context.Estates.Where(e => e.UserId == userId).ToList();
        foreach(EstateModel estate in list)
            estate.User = null; // To ensure that cycle User.EstateMode.User.Estatemodel... wont happen
        return list;
    }
    public List<EstateModel> GetEstatesListByUserId(int userId, int statusId)
    {
        List<EstateModel> list = _context.Estates.Where(e => e.UserId == userId && e.StatusId == statusId).ToList();
        foreach(EstateModel estate in list)
            estate.User = null; // To ensure that cycle User.EstateMode.User.Estatemodel... wont happen

        return list;
    }
    public async Task AddEstate(NewEstateModel estate)
    {
        string photoPath = await UploadImage(estate.File);
        var newEstate = new EstateModel()
        {
            Address = estate.Address,
            RoomsNum = estate.RoomsNum,
            Area = estate.Area,
            Price = estate.Price,
            StatusId = estate.StatusId,
            Notes = estate?.Notes!,
            Photo = photoPath,
            UserId = estate.UserId
        };

        await _context.Estates.AddAsync(newEstate);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveEstateById(int estateId)
    {
        _context.Estates.Remove(new EstateModel(){Id = estateId});
        await _context.SaveChangesAsync();
    }


    public async Task<string> UploadImage(IFormFile photo)
    {
        try
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
            var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/uploads", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            return fileName + ".png";
        }
        catch
        {
            return null!;
        }
    }


    
    public async Task ChangeStatus(int estateId, int newStatusId)
    {
        EstateModel estate = _context.Estates.FirstOrDefault(est => est.Id == estateId)!;
        if (estate is null)
            throw new Exception("Estate not found by id");

        estate.StatusId = newStatusId;
        await _context.SaveChangesAsync();
    }
        // throw new NotImplementedException();

}
