﻿using System.Linq;
using Microsoft.Phone.Data.Linq;
using PhoneGuitarTab.Data;

namespace PhoneGuitarTab.UI.Data
{
    /// <summary>
    /// Provides the way to update database schema and content to the latest version incrementally
    /// </summary>
    public class UpdateScript
    {
        private readonly int _dbVersion;
        private readonly string _dbConnectionString;
        private readonly IDataContextService _dbService;

        public UpdateScript(IDataContextService dbService, string dbConnectionString, int dbVersion)
        {
            _dbService = dbService;
            _dbConnectionString = dbConnectionString;
            _dbVersion = dbVersion;
        }

        public void CheckAndUpdate()
        {
            TabDataContext dataContext = new TabDataContext(_dbConnectionString);
            DatabaseSchemaUpdater dbUpdater = dataContext.CreateDatabaseSchemaUpdater();

            if (dbUpdater.DatabaseSchemaVersion < _dbVersion)
                UpdateDataBase(dbUpdater);

            
        }

        private void UpdateDataBase(DatabaseSchemaUpdater dbUpdater)
        {
            CheckAndUpdateToVersion1_1(dbUpdater);

            CheckAndUpdateToVersion2_0(dbUpdater);

            CheckAndUpdateToVersion3_0(dbUpdater);

            _dbService.SubmitChanges();

            // Change database version.
            dbUpdater.DatabaseSchemaVersion = _dbVersion;

            // Perform the database update in a single transaction.
            dbUpdater.Execute();

            //Update table data - for updated db.
            UpdateRowsForVersion3_0();
            _dbService.SubmitChanges();
           
        }

        #region Release 1.1
        private void CheckAndUpdateToVersion1_1(DatabaseSchemaUpdater dbUpdater)
        {
            if (!_dbService.TabTypes.Any(type => type.Name == "chords"))
                _dbService.TabTypes.InsertOnSubmit(new TabType() { Name = "chords", ImageUrl = "/Images/instrument/Chords" });

            if (!_dbService.TabTypes.Any(type => type.Name == "drums"))
                _dbService.TabTypes.InsertOnSubmit(new TabType() { Name = "drums", ImageUrl = "/Images/instrument/Drums" });
        }
        #endregion

        #region Release 2.0
        
        private void CheckAndUpdateToVersion2_0(DatabaseSchemaUpdater dbUpdater)
        {
            // Release 2.0
            if (!_dbService.TabTypes.Any(type => type.Name == Strings.GuitarPro))
                _dbService.TabTypes.InsertOnSubmit(new TabType() { Name = Strings.GuitarPro, ImageUrl = "/Images/instrument/Guitarpro" });
        }

        #endregion

        #region Release 3.0

        private void CheckAndUpdateToVersion3_0(DatabaseSchemaUpdater dbUpdater)
        {
               // Release 3.0
            if (dbUpdater.DatabaseSchemaVersion > 0 && dbUpdater.DatabaseSchemaVersion < 3)
            {
                dbUpdater.AddColumn<Tab>("CloudName");
                dbUpdater.AddColumn<Tab>("AlbumCoverImageUrl");

                dbUpdater.AddColumn<Group>("LargeImageUrl");
                dbUpdater.AddColumn<Group>("ExtraLargeImageUrl");
                _dbService.TabTypes.InsertOnSubmit(new TabType() { Name = Strings.MusicXml, ImageUrl = "/Images/instrument/MusicXML" });

                //update the tab type image urls.
                _dbService.TabTypes.Single(tt => tt.Name == "tab").ImageUrl = "/Images/instrument/Electric-Guitar";
                _dbService.TabTypes.Single(tt => tt.Name == "bass").ImageUrl = "/Images/instrument/Bass";
                _dbService.TabTypes.Single(tt => tt.Name == "chords").ImageUrl = "/Images/instrument/Chords";
                _dbService.TabTypes.Single(tt => tt.Name == "drums").ImageUrl = "/Images/instrument/Drums";
                _dbService.TabTypes.Single(tt => tt.Name == Strings.GuitarPro).ImageUrl = "/Images/instrument/Guitarpro";
            
            }
        }

        private void UpdateRowsForVersion3_0()
        {

            foreach (Group g in _dbService.Groups)
            {
                _dbService.Groups.Single(group => group.Id == g.Id).ImageUrl = "/Images/light/groupDefault.png";
            }

        }

        #endregion
    }
}
