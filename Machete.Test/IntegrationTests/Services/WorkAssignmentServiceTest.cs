﻿#region COPYRIGHT
// File:     WorkAssignmentServiceTest.cs
// Author:   Savage Learning, LLC.
// Created:  2012/06/17 
// License:  GPL v3
// Project:  Machete.Test
// Contact:  savagelearning
// 
// Copyright 2011 Savage Learning, LLC., all rights reserved.
// 
// This source file is free software, under either the GPL v3 license or a
// BSD style license, as supplied with this software.
// 
// This source file is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the license files for details.
//  
// For details please refer to: 
// http://www.savagelearning.com/ 
//    or
// http://www.github.com/jcii/machete/
// 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Machete.Data;
using Machete.Data.Infrastructure;
using Machete.Domain;
using Machete.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using Machete.Web.Helpers;

namespace Machete.Test
{
    [TestClass]
    public class WorkAssignmentServiceTest 
    {
        viewOptions dOptions;
        FluentRecordBase frb;

        [TestInitialize]
        public void TestInitialize()
        {
            frb = new FluentRecordBase();
            frb.Initialize(new TestInitializer(), "macheteConnection");
            dOptions = new viewOptions
            {
                CI = new CultureInfo("en-US", false),
                sSearch = "",
                date = DateTime.Today,
                dwccardnum = null,
                woid = null,
                orderDescending = false,
                sortColName = "WOID",
                displayStart = 0,
                displayLength = 20
            };

        }
        [TestCleanup]
        public void TestCleanup()
        {
            //frb.DB.Database.Delete();
            frb.Dispose();
            frb = null;
        }


        [TestMethod]
        public void Integration_WA_Service_GetIndexView_basic()
        {   
            //
            // Arrange
            var desc = "DESCRIPTION " + frb.RandomString(20);
            frb.AddWorkAssignment(desc: desc).AddWorkAssignment(desc: desc);
            frb.AddWorkAssignment(desc: desc).AddWorkAssignment(desc: desc);
            frb.AddWorkAssignment(desc: desc).AddWorkAssignment(desc: desc);
            frb.AddWorkAssignment(desc: desc).AddWorkAssignment(desc: desc);
            dOptions.sSearch = desc;
            //
            //Act
            var result = frb.ToServWorkAssignment().GetIndexView(dOptions);
            //
            //Assert
            var tolist = result.query.ToList();
            Assert.IsNotNull(result, "return value is null");
            Assert.IsInstanceOfType(result, typeof(dataTableResult<WorkAssignment>));
            Assert.AreEqual(8, result.query.Count()); //pending excluded
        }
        [TestMethod]
        public void Integration_WA_Service_GetIndexView_check_workerjoin_blank_worker_ok()
        {
            dOptions.sortColName = "assignedWorker";
            var result = frb.ToServWorkAssignment().GetIndexView(dOptions);
            var tolist = result.query.ToList();
            Assert.IsNotNull(tolist, "return value is null");
            Assert.IsInstanceOfType(result, typeof(dataTableResult<WorkAssignment>));
            Assert.AreEqual(8, result.query.Count()); //pending excluded
        }
        [TestMethod]
        public void Integration_WA_Service_GetIndexView_checkwoidfilter()
        {
            //Act
            dOptions.woid = 1;
            dOptions.orderDescending = true;
            var result = frb.ToServWorkAssignment().GetIndexView(dOptions);
            //
            //Assert
            var tolist = result.query.ToList();
            Assert.IsNotNull(tolist, "return value is null");
            Assert.IsInstanceOfType(result, typeof(dataTableResult<WorkAssignment>));
            Assert.AreEqual(3, result.query.Count());
            Assert.AreEqual(10, frb.ToServWorkAssignment().TotalCount());
        }
        [TestMethod]
        public void Integration_WA_Service_GetIndexView_check_search_paperordernum()
        { 
            //
            // arrange
            frb.AddWorkOrder().AddWorkAssignment().AddWorkAssignment().AddWorkAssignment();
            frb.AddWorkOrder(paperordernum: 12420).AddWorkAssignment().AddWorkAssignment();
            //
            //Act
            dOptions.sSearch = "12420";
            dOptions.orderDescending = true;
            var result = frb.ToServWorkAssignment().GetIndexView(dOptions);
            //
            //Assert
            var tolist = result.query.ToList();
            Assert.IsNotNull(tolist, "return value is null");
            Assert.IsInstanceOfType(result, typeof(dataTableResult<WorkAssignment>));
            Assert.AreEqual(12420, tolist[0].workOrder.paperOrderNum);
            Assert.AreEqual(2, result.query.Count());
            Assert.AreEqual(5, frb.ToServWorkAssignment().TotalCount());
        }
        [TestMethod]
        public void Integration_WA_Service_GetIndexView_check_search_description()
        {
            //
            //Act
            dOptions.sSearch = "foostring1";
            dOptions.woid = 1;
            dOptions.orderDescending = true;
            var serv = frb.ToServWorkAssignment();
            var result = frb.AddWorkAssignment(desc: "foostring1").ToServWorkAssignment().GetIndexView(dOptions);
            //
            //Assert
            var tolist = result.query.ToList();
            Assert.IsNotNull(tolist, "return value is null");
            Assert.IsInstanceOfType(result, typeof(dataTableResult<WorkAssignment>));
            Assert.AreEqual("foostring1", tolist[0].description);
            Assert.AreEqual(1, result.query.Count());
        }
        [TestMethod]
        public void Integration_WA_Service_GetIndexView_check_search_Updatedby()
        {
            var updatedby = "foostring1";
            frb.AddWorkAssignment();
            frb.AddWorkAssignment(updatedby: updatedby);
            dOptions.sSearch = updatedby;
            dOptions.orderDescending = true;
            var result = frb.ToServWorkAssignment().GetIndexView(dOptions);
            //
            //Assert
            var tolist = result.query.ToList();
            Assert.IsNotNull(tolist, "return value is null");
            Assert.IsInstanceOfType(result, typeof(dataTableResult<WorkAssignment>));
            Assert.AreEqual(updatedby, tolist[0].Updatedby);
            Assert.AreEqual(1, result.query.Count());
            Assert.AreEqual(2, frb.ToServWorkAssignment().TotalCount());
        }
        [TestMethod]
        public void Integration_WA_Service_GetIndexView_check_search_skill()
        {
            dOptions.sSearch = "Digging";
            dOptions.orderDescending = true;
            var result = frb.ToServWorkAssignment().GetIndexView(dOptions);
            //
            //Assert
            var tolist = result.query.ToList();
            Assert.IsNotNull(tolist, "return value is null");
            Assert.IsInstanceOfType(result, typeof(dataTableResult<WorkAssignment>));
            Assert.AreEqual(70, tolist[0].skillID); //ID=70 is Digging
            Assert.AreEqual(1, result.query.Count());
            Assert.AreEqual(10, frb.ToServWorkAssignment().TotalCount());
        }
        [TestMethod]
        public void Integration_WA_Service_GetIndexView_check_searchdateTimeofWork()
        {
            //
            //Act
            dOptions.sSearch = DateTime.Today.AddHours(9).ToString();
            dOptions.orderDescending = true;
            var result = frb.ToServWorkAssignment().GetIndexView(dOptions);
            //
            //Assert
            var tolist = result.query.ToList();
            Assert.IsNotNull(tolist, "return value is null");
            Assert.IsInstanceOfType(result, typeof(dataTableResult<WorkAssignment>));
            Assert.AreEqual(70, tolist[0].skillID);
            Assert.AreEqual(3, result.query.Count());
            Assert.AreEqual(10, frb.ToServWorkAssignment().TotalCount());
        }
        //
        // Simulates doubleclicking on a worker in the workerSignin list
        // 
        [TestMethod]
        public void Integration_WA_Service_GetIndexView_check_searchdwccardnum()
        {
            //arrange
            //var skill = LookupCache.getSingleEN("skill","painter (rollerbrush)");
            var skill = 61;
            var w = frb.AddWorker(skill1: skill).ToWorker();
            dOptions.dwccardnum = w.dwccardnum;
            dOptions.orderDescending = true;
            frb.AddWorkAssignment(skill: skill);
            //Act
            var result = frb.ToServWorkAssignment().GetIndexView(dOptions);
            //
            //Assert
            var tolist = result.query.ToList();
            Assert.IsNotNull(tolist, "return value is null");
            Assert.IsInstanceOfType(result, typeof(dataTableResult<WorkAssignment>));
            Assert.AreEqual(1, result.query.Count());
        }
        [TestMethod]
        public void Integration_WA_Service_GetIndexView_check_requested_filter()
        {
            //Arrange
            frb.AddWorkerRequest().AddWorkAssignment();
            //Act
            dOptions.orderDescending = true;
            dOptions.wa_grouping = "requested";
            var result = frb.ToServWorkAssignment().GetIndexView(dOptions);
            //
            //Assert
            var tolist = result.query.ToList();
            Assert.IsNotNull(tolist, "return value is null");
            Assert.IsInstanceOfType(result, typeof(dataTableResult<WorkAssignment>));
            //Assert.AreEqual(61, tolist[0].skillID);
            Assert.AreEqual(1, result.query.Count());
        }
        [TestMethod]
        public void Integration_WA_Service_Assign_updates_WSI_and_WA()
        {
            var wsi1 = frb.ToWorkerSignin();
            var wa1 = frb.ToWorkAssignment();
            var result = frb.ToServWorkAssignment().Assign(wa1, wsi1, "test script");
            var wsi2 = frb.ToWorkerSignin();
            var wa2 = frb.ToWorkAssignment();
            Assert.IsNotNull(result);
            Assert.IsNotNull(wa2.workerAssignedID);
            Assert.IsNotNull(wa2.workerSigninID);
            Assert.IsNotNull(wsi2.WorkAssignmentID);
            Assert.IsNotNull(wsi2.WorkerID);
        }
        [TestMethod]
        public void Integration_WA_Service_GetSummary()
        {
            var result = frb.ToServWorkAssignment().GetSummary("");
            Assert.IsNotNull(result, "Person.ID is Null");
        }
        [TestMethod]
        public void Integration_WA_Service_Delete_removes_record()

        {
            Assert.AreEqual(0, frb.ToServWorkAssignment().GetAll().Count());
            frb.AddWorkAssignment().AddWorkAssignment().AddWorkAssignment().AddWorkAssignment().AddWorkAssignment();
            frb.AddWorkAssignment().AddWorkAssignment().AddWorkAssignment().AddWorkAssignment().AddWorkAssignment();

            var before = frb.ToServWorkAssignment().GetAll();
            Assert.AreEqual(10, before.Count());
            frb.ToServWorkAssignment().Delete(frb.ToWorkAssignment().ID, "Intg Test");
            var after = frb.ToServWorkAssignment().GetAll();
            Assert.AreEqual(9, after.Count());
            Assert.AreNotSame(before.Count(), after.Count());
        }
    }
}