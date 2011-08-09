﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Machete.Data;
using Machete.Data.Infrastructure;
using Machete.Domain;
using Machete.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity.Database;
using System.Data.Entity.Validation;

namespace Machete.Test
{
    [TestClass]
    public class WorkerSigninServiceTest
    {

        WorkerSigninRepository _wsiRepo;
        WorkerRepository _wRepo;
        PersonRepository _pRepo;
        ImageRepository _iRepo;
        DatabaseFactory _dbFactory;
        WorkerSigninService _wsiServ;
        WorkerService _wServ;
        PersonService _pServ;
        ImageService _iServ;
        IUnitOfWork _unitofwork;
        MacheteContext MacheteDB;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) { }

        [TestInitialize]
        public void TestInitialize()
        {
            _dbFactory = new DatabaseFactory();
            _iRepo = new ImageRepository(_dbFactory);
            _wsiRepo = new WorkerSigninRepository(_dbFactory);
            _pRepo = new PersonRepository(_dbFactory);
            _unitofwork = new UnitOfWork(_dbFactory);
            _pServ = new PersonService(_pRepo, _unitofwork);
            _iServ = new ImageService(_iRepo, _unitofwork);
            _wsiServ = new WorkerSigninService(_wsiRepo, _wRepo, _pRepo, _iRepo, _unitofwork);
        }

        [TestMethod]
        public void DbSet_TestMethod4()
        {                        
            IEnumerable<WorkerSignin> filteredWSI = _wsiServ.GetWorkerSignins().Where(jj => jj.dateforsignin.Date.Equals(Convert.ToDateTime("08/02/2011")));
            Assert.IsNotNull(filteredWSI, "Person.ID is Null");
        }
    }
}