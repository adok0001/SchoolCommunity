using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using assignment2.Data;
using assignment2.Models;
using assignment2.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs;

namespace assignment2.Controllers
{
    public class AdvertisementsController : Controller
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly SchoolCommunityContext _context;
        private readonly string adContainerName = "adimages";


        public AdvertisementsController(SchoolCommunityContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        // GET: Advertisements/Id
        public async Task<IActionResult> Index(string id) {
              if(id==null){
                   return NotFound();
               }
            
             var advertisement = await _context.Advertisements.ToListAsync();

            var viewModel = new AdsViewModel();
            viewModel.Community = _context.Communities.Where(x => x.Id == id).Single();
            viewModel.Advertisements = advertisement.Where(a => a.CommunityId == id);
            

            return View(viewModel);
        }

        // GET: Advertisements/Create
        public IActionResult Create(string id)
        {
            var viewModel = new AdsViewModel();
            viewModel.Community = _context.Communities.Where(x => x.Id == id).Single();
            return View(viewModel);
        }

  
        // POST: Advertisements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, string id)
        {
            var viewModel = new AdsViewModel();
            viewModel.Community = _context.Communities.Where(x => x.Id == id).Single();
                       viewModel.Advertisements = _context.Advertisements.Where(a => a.CommunityId == id);

            BlobContainerClient containerClient;
            // Create the container and return a container client object
            try
            {
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(adContainerName);
                // Give access to public
                containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (Azure.RequestFailedException)
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(adContainerName);
            }


            try
            {
                string randomFileName = System.IO.Path.GetRandomFileName();
                // create the blob to hold the data
                var blockBlob = containerClient.GetBlobClient(randomFileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                using (var memoryStream = new System.IO.MemoryStream())
                {
                    // copy the file data into memory
                    await file.CopyToAsync(memoryStream);

                    // navigate back to the beginning of the memory stream
                    memoryStream.Position = 0;

                    // send the file to the cloud
                    await blockBlob.UploadAsync(memoryStream);
                    memoryStream.Close();
                }

                // add the photo to the database if it uploaded successfully
                var image = new Advertisement
                {
                    Url = blockBlob.Uri.AbsoluteUri,
                    FileName = randomFileName,
                    CommunityId = id
                };

                _context.Advertisements.Add(image);
                _context.SaveChanges();
            }
            catch (Azure.RequestFailedException)
            {
                View("Error");
            }
             
            return View("Index",viewModel);
        }

        // GET: Advertisements/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var viewModel = new AdsViewModel();
            var advertisement = await _context.Advertisements
                .ToListAsync();
            if (advertisement == null)
            {
                return NotFound();
            }

            viewModel.Ad = advertisement.FirstOrDefault(a => a.Id == id);
            viewModel.Community = _context.Communities.Where(x => x.Id == viewModel.Ad.CommunityId).Single();
            return View(viewModel);
        }

        // POST: Advertisements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!AdvertisementExists(id)) { return BadRequest(); }
            var Advertisement = await _context.Advertisements.FindAsync(id);

            
            var advertisement = await _context.Advertisements.ToListAsync();
                var viewModel = new AdsViewModel
            {
                Ad = Advertisement,
                Community = _context.Communities.Where(x => x.Id == Advertisement.CommunityId).Single(),
                Advertisements = advertisement.Where(a => a.CommunityId == Advertisement.CommunityId)
            };

            BlobContainerClient containerClient;
            // Get the container and return a container client object
            try
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(adContainerName);
            }
            catch (Azure.RequestFailedException)
            {
                return View("Error");
            }

            try
            {
                // Get the blob that holds the data
                var blockBlob = containerClient.GetBlobClient(Advertisement.FileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                _context.Advertisements.Remove(Advertisement);
                await _context.SaveChangesAsync();

            }
            catch (Azure.RequestFailedException)
            {
                return View("Error");
            }

            return View("Index",viewModel);
        }

        private bool AdvertisementExists(string id)
        {
            return _context.Advertisements.Any(e => e.Id == id);
        }
    }
}
