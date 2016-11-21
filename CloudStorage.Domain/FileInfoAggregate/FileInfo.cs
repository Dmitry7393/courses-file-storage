namespace CloudStorage.Domain.FileAggregate
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents a domain model of file.
    /// </summary>
    public class FileInfo
    {

        private const string BASE_EXTENSION = "dat";

        private string _name;

        /// <summary>
        /// Gets or sets the identifier of game.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating where name of file.
        /// </summary>
        /// <value>Name of file.</value>
        public string Name
        {
            get
            {
                return this._name;
            }

            set
            {
                if (!FileInfoValidation.IsValidName(value))
                {
                    throw new ArgumentException("Wrong name of file");
                }

                this._name = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating where date of creation.
        /// </summary>
        /// <value>Creation date.</value>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating where extension of file.
        /// </summary>
        /// <value>Extension of file.</value>
        public string Extension { get; set; }

        /// <summary>
        /// Gets or sets a value indicating where owher id.
        /// </summary>
        public string OwnerId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating location of file
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating where users.
        /// </summary>
        /// <value>Users.</value>
        public virtual ICollection<User> Users { get; set; }

        /// <summary>
        /// Gets a value indicating where path to file.
        /// </summary>
        [NotMapped]
        public string PathToFile
        {
            get 
            {
                return Path.Combine(this.OwnerId, this.Id + "." + BASE_EXTENSION);
            }
        }

        /// <summary>
        /// Gets a value indicating where full name of file including extension.
        /// </summary>
        [NotMapped]
        public string FullName
        {
            get
            {
                return String.Format("{0}{1}", this.Name, this.Extension);
            }
        }
    }
}
