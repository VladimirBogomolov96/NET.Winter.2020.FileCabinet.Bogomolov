﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short height, decimal income, char patronymicLetter)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(nameof(firstName), "First name can not be null.");
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("First name length must be from 2 to 60 chars.", nameof(firstName));
            }

            if (firstName.Trim().Length == 0)
            {
                throw new ArgumentException("First name can not contain only whitespaces.", nameof(firstName));
            }

            if (lastName is null)
            {
                throw new ArgumentNullException(nameof(lastName), "Last name can not be null.");
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException("Last name length must be from 2 to 60 chars.", nameof(lastName));
            }

            if (lastName.Trim().Length == 0)
            {
                throw new ArgumentException("Last name can not contain only whitespaces.", nameof(lastName));
            }

            if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Today)
            {
                throw new ArgumentException("Date of birth must be from 01-Jan-1950 to today.", nameof(dateOfBirth));
            }

            if (height <= 0 || height > 300)
            {
                throw new ArgumentException("Height must be from 1 to 300 cm.", nameof(height));
            }

            if (income < 0)
            {
                throw new ArgumentException("Income must be not negative number.", nameof(income));
            }

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Height = height,
                Income = income,
                PatronymicLetter = patronymicLetter,
            };

            this.list.Add(record);

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            FileCabinetRecord[] fileCabinetRecords = this.list.ToArray();
            return fileCabinetRecords;
        }

        public int GetStat()
        {
            return this.list.Count;
        }
    }
}