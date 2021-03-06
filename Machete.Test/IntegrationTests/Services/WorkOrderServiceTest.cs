﻿#region COPYRIGHT
// File:     WorkOrderServiceTest.cs
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
    public class WorkOrderServiceTest 
    {
        viewOptions dOptions;
        FluentRecordBase frb;

        [TestInitialize]
        public void TestInitialize()
        {
            frb = new FluentRecordBase();
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

        [TestMethod, TestCategory(TC.IT), TestCategory(TC.Service), TestCategory(TC.WorkOrders)]
        public void Integration_WO_Service_GetSummary()
        {
            //
            //Arrange
            var Date = frb.ToServWorkOrder().GetSummary().OrderByDescending(o => o.date).First().date.Value.AddDays(1);
            frb.AddWorkOrder(status: WorkOrder.iCancelled, dateTimeOfWork: Date).AddWorkAssignment()
               .AddWorkOrder(status: WorkOrder.iPending, dateTimeOfWork: Date).AddWorkAssignment()
               .AddWorkOrder(status: WorkOrder.iCompleted, dateTimeOfWork: Date).AddWorkAssignment()
               .AddWorkOrder(status: WorkOrder.iCompleted, dateTimeOfWork: Date).AddWorkAssignment()
               .AddWorkOrder(status: WorkOrder.iActive, dateTimeOfWork: Date).AddWorkAssignment()
               .AddWorkOrder(status: WorkOrder.iActive, dateTimeOfWork: Date).AddWorkAssignment();
            //
            //Act
            IEnumerable<WorkOrderSummary> result = frb.ToServWorkOrder().GetSummary(Date.ToShortDateString()).ToList();
            //
            //Assert
            Assert.IsNotNull(result, "GetSummary result is Null");
            Assert.AreEqual(2, result.Where(r => r.status == WorkOrder.iActive).First().count, "GetSummary returned incorrect number of Active records");
            Assert.IsTrue(result.Where(r => r.status == WorkOrder.iPending).First().count == 1, "GetSummary returned incorrect number of Pending records");
            Assert.IsTrue(result.Where(r => r.status == WorkOrder.iCompleted).First().count == 2, "GetSummary returned incorrect number of Completed records");
            Assert.IsTrue(result.Where(r => r.status == WorkOrder.iCancelled).First().count == 1, "GetSummary returned incorrect number of Cancelled records");
        }
        [TestMethod, TestCategory(TC.IT), TestCategory(TC.Service), TestCategory(TC.WorkOrders)]
        public void Integration_WO_Service_CombinedSummary()
        {
            //
            // Arrange
            string search = "";
            bool orderdescending = true;
            int displayStart = 0;
            int displayLength = 50;
            var Date = frb.ToServWorkOrder().GetSummary().OrderByDescending(o => o.date).First().date.Value.AddDays(1);
            frb.AddWorkOrder(status: WorkOrder.iCancelled, dateTimeOfWork: Date).AddWorkAssignment()
               .AddWorkOrder(status: WorkOrder.iPending, dateTimeOfWork: Date).AddWorkAssignment().AddWorkAssignment()
               .AddWorkOrder(status: WorkOrder.iCompleted, dateTimeOfWork: Date).AddWorkAssignment().AddWorkAssignment()
               .AddWorkOrder(status: WorkOrder.iCompleted, dateTimeOfWork: Date).AddWorkAssignment()
               .AddWorkOrder(status: WorkOrder.iActive, dateTimeOfWork: Date).AddWorkAssignment().AddWorkAssignment()
               .AddWorkOrder(status: WorkOrder.iActive, dateTimeOfWork: Date).AddWorkAssignment().AddWorkAssignment();
            //
            //Act
            dataTableResult<WOWASummary> result = frb.ToServWorkOrder().CombinedSummary(search, orderdescending, displayStart, displayLength);
            WOWASummary wowa = result.query.First();
            //
            //Assert
            Assert.IsNotNull(result, "CombinedSummary.IEnumerable is Null");
            Assert.IsNotNull(wowa, "CombinedSummary.IEnumerable.query is null");
            Assert.AreEqual(4, wowa.active_wa, "CombinedSummary returned unexpected active_wa value");
            Assert.AreEqual(2, wowa.active_wo, "CombinedSummary returned unexpected active_wo value");
            Assert.AreEqual(1, wowa.cancelled_wa, "CombinedSummary returned unexpected cancelled_wa value");
            Assert.AreEqual(1, wowa.cancelled_wo, "CombinedSummary returned unexpected cancelled_wo value");
            Assert.AreEqual(3, wowa.completed_wa, "CombinedSummary returned unexpected completed_wa value");
            Assert.AreEqual(2, wowa.completed_wo, "CombinedSummary returned unexpected completed_wo value");
            Assert.AreEqual(0, wowa.expired_wa, "CombinedSummary returned unexpected expired_wa value");
            Assert.AreEqual(0, wowa.expired_wo, "CombinedSummary returned unexpected expired_wo value");
            Assert.AreEqual(2, wowa.pending_wa, "CombinedSummary returned unexpected pending_wa value");
            Assert.AreEqual(1, wowa.pending_wo, "CombinedSummary returned unexpected pending_wo value");

        }
        [TestMethod, TestCategory(TC.IT), TestCategory(TC.Service), TestCategory(TC.WorkOrders)]
        public void Integration_WO_Service_get_GroupView()
        {
            // Arrange
            frb.AddWorkOrder(status: WorkOrder.iActive, dateTimeOfWork: DateTime.Now).AddWorkAssignment();
            //
            //Act
            var result = frb.ToServWorkOrder().GetActiveOrders(DateTime.Now, assignedOnly:false);
            Assert.IsNotNull(result, "Person.ID is Null");
        }
        [TestMethod, TestCategory(TC.IT), TestCategory(TC.Service), TestCategory(TC.WorkOrders)]
        public void Integration_WO_Service_GetIndexView()
        {
            //
            //Arrange
            var Date = frb.ToServWorkOrder().GetSummary().OrderByDescending(a => a.date).First().date.Value.AddDays(1);

            frb.AddWorkOrder(dateTimeOfWork: Date).AddWorkOrder(dateTimeOfWork: Date).AddWorkOrder(dateTimeOfWork: Date);
            frb.AddWorkOrder(dateTimeOfWork: Date).AddWorkOrder(dateTimeOfWork: Date).AddWorkOrder(dateTimeOfWork: Date);
            viewOptions o = new viewOptions();
            o.CI = new CultureInfo("en-US", false);
            o.sSearch = Date.ToShortDateString();
            o.EmployerID = null;
            o.status = null;
            o.orderDescending = true;
            o.displayStart = 0;
            o.displayLength = 20;
            o.sortColName = "WOID";
            //
            //Act
            dataTableResult<WorkOrder> result = frb.ToServWorkOrder().GetIndexView(o);
            //
            //Assert
            IEnumerable<WorkOrder> query = result.query.ToList();
            Assert.IsNotNull(result, "IEnumerable is Null");
            Assert.IsNotNull(query, "IEnumerable.query is null");
            Assert.AreEqual(6, query.Count(), "Expected 6, but GetIndexView returned {0} records", query.Count());

        }
    }
}
