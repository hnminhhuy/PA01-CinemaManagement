﻿using CinemaManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaManagement.ViewModels
{
    public class MovieCommand : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public RelayCommand DeleteCommand { get; set; }
        public Movie movie { get; set; }

        public MovieCommand(Movie _movie, RelayCommand _deleteCommand)
        {
            this.movie = _movie;
            this.DeleteCommand = _deleteCommand;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }

    public class MovieViewModel : INotifyPropertyChanged
    {

        private readonly DbCinemaManagementContext _context;



        //public List<Movie> GetMovies()
        //{
        //    return _context.Movies.ToList();
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<MovieCommand> MoviesList { get; set; }
        private MovieCommand _selectedMovie;

        public MovieCommand SelectedMovie
        {
            get { return _selectedMovie; }
            set
            {
                if (_selectedMovie != value)
                {
                    _selectedMovie = value;
                    OnPropertyChanged(nameof(SelectedMovie));
                }
            }
        }
        public int SelectedIndexMovie { get; set; }
        private ICollection<Genre> GenreList { get; set; }
        private ICollection<AgeCertificate> ageCertificates { get; set; }
        public RelayCommand DeleteCommand { get; }
        //public RelayCommand EditCommand { get; set; }

        public MovieViewModel()
        {

            //GenerateGenreData();
            //GenerateAgeCertificate();
            //MoviesList = GenerateSampleData(DeleteCommand);

            //DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            //SelectedMovie = MoviesList[0];

        }
        public MovieViewModel(DbCinemaManagementContext context)
        {
            _context = context;
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            MoviesList = GenerateSampleData(DeleteCommand);
            SelectedMovie = MoviesList[0];
            Debug.WriteLine(MoviesList[0].movie.Title);
            Debug.WriteLine(MoviesList[0].movie.PosterPath);

        }

        private async Task DeleteMovieAsync(Movie movie)
        {
            try
            {
                // Remove all contributors associated with the movie
                _context.Contributors.RemoveRange(movie.Contributors);

                // Remove all show times associated with the movie
                _context.ShowTimes.RemoveRange(movie.ShowTimes);

                // Clear the genres associated with the movie
                movie.Genres.Clear();

                // Save changes to the database
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();

             
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting movie related data: {ex.Message}");
            }
        }

        private bool CanDelete(object parameter)
        {

            return SelectedMovie != null;
        }

        private void OnDelete(object obj)
        {
            // Print a debug message to indicate that the OnDelete method is being called
            Debug.WriteLine("OnDelete method called.");

            // Check if SelectedMovie is correctly set
            if (SelectedMovie != null)
            {
                Debug.WriteLine($"Deleting movie: {SelectedMovie.movie.Title}");
                DeleteMovieAsync(SelectedMovie.movie);
                MoviesList.Remove(SelectedMovie);
                // Remove the selected movie from the MoviesList
            }
            else
            {
                // Print a debug message if SelectedMovie is null
                Debug.WriteLine("SelectedMovie is null. Cannot delete.");
            }

        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void GenerateGenreData()
        {
            if (GenreList == null)
            {
                GenreList = new List<Genre>
                {
                    new Genre { GenreId = 1, GenreName = "Action" },
                    new Genre { GenreId = 2, GenreName = "Adventure" },
                    new Genre { GenreId = 3, GenreName = "Comedy" },
                };
            }
        }
        private void GenerateAgeCertificate()
        {
            if (ageCertificates == null)
            {
                ageCertificates = new List<AgeCertificate>
                {
                    new AgeCertificate { AgeCertificateId = 1, DisplayContent = "C13", RequireAge = 13, ForegroundColor = "Orange", BackgroundColor = "Transparent"},
                    new AgeCertificate { AgeCertificateId = 2, DisplayContent = "C18", RequireAge = 18, ForegroundColor = "Red", BackgroundColor = "Transparent"},
                    new AgeCertificate { AgeCertificateId = 3, DisplayContent = "P", RequireAge = 0, ForegroundColor = "Green", BackgroundColor = "Transparent"},
                };
            }
        }

        private ObservableCollection<MovieCommand> GenerateSampleData(RelayCommand DeleteCommand)
        {

            //List<Genre> avatarGenres = new List<Genre>
            //{
            //    GenreList.FirstOrDefault(g => g.GenreId == 1), // Action
            //    GenreList.FirstOrDefault(g => g.GenreId == 2), // Adventure
            //    GenreList.FirstOrDefault(g => g.GenreId == 3)  // Comedy
            //};

            //List<Genre> duneGenres = new List<Genre>
            //{
            //    GenreList.FirstOrDefault(g => g.GenreId == 1), // Action
            //    GenreList.FirstOrDefault(g => g.GenreId == 2), // Adventure
            //};
            //List<Genre> pandaGenres = new List<Genre>
            //{
            //    GenreList.FirstOrDefault(g => g.GenreId == 2), // Adventure
            //};

            // Generate sample data for G
            //List<Movie> movies = GetMovies();
            ObservableCollection<MovieCommand> res = new ObservableCollection<MovieCommand>();

            var movies = _context.Movies
                .Include(m => m.Genres)
                .ToList();

            foreach (var movie in movies)
            {
                res.Add(new MovieCommand(movie, DeleteCommand));
            }

            return res;


            //res.Add(new MovieCommand(new Movie
            //{
            //    MovieId=1,
            //    Title = "Avatar: The way of water",
            //    Duration = 120,
            //    PublishYear = 2022,
            //    Imdbrating = 7.8,
            //    AgeCertificateId = 1,
            //    AgeCertificate = ageCertificates.ElementAt(0),
            //    PosterPath = "/Assets/Images/Poster/avatar_the_way_of_water.jpg",
            //    Description = "\"Avatar: The Way of Water\" is a sequel to the first \"Avatar\" film, set more than a decade after the events of the first film1. The story follows the Sully family (Jake, Neytiri, and their kids) as they seek refuge with the aquatic Metkayina clan of Pandora, a habitable exomoon on which they live2",
            //    TrailerPath = "/Assets/Videos/avatar.mp4",
            //    Genres = avatarGenres

            //}, DeleteCommand));
            //res.Add(new MovieCommand(new Movie
            //{
            //    MovieId = 2,
            //    Title = "Dune Part Two",
            //    Duration = 120,
            //    PublishYear = 2023,
            //    Imdbrating = 7.8,
            //    AgeCertificateId = 2,
            //    AgeCertificate = ageCertificates.ElementAt(1),
            //    PosterPath = "/Assets/Images/Poster/dune2.jpg",
            //    Description = "Dune 2 is the sequel to Dune (2021)1. It is the second of a two-part adaptation of the 1965 novel Dune by Frank Herbert1. The movie follows Paul Atreides as he unites with the Fremen people of the desert planet Arrakis to wage war against House Harkonnen1",
            //    TrailerPath = "/Assets/Videos/dune2.mp4",
            //    Genres = duneGenres

            //}, DeleteCommand));
            //res.Add(new MovieCommand(new Movie
            //{
            //    MovieId = 3,
            //    Title = "Avatar: The way of water",
            //    Duration = 120,
            //    PublishYear = 2022,
            //    Imdbrating = 7.8,
            //    AgeCertificateId = 1,
            //    AgeCertificate = ageCertificates.ElementAt(0),
            //    PosterPath = "/Assets/Images/Poster/avatar_the_way_of_water.jpg",
            //    Description = "\"Avatar: The Way of Water\" is a sequel to the first \"Avatar\" film, set more than a decade after the events of the first film1. The story follows the Sully family (Jake, Neytiri, and their kids) as they seek refuge with the aquatic Metkayina clan of Pandora, a habitable exomoon on which they live2",
            //    TrailerPath = "/Assets/Videos/avatar.mp4",
            //    Genres = pandaGenres

            //}, DeleteCommand));




        }



    }
    public class GenreViewConverter : IValueConverter
    {
        public class CombinedItem
        {
            public String GenreName { get; set; }
            public String Icon { get; set; }
        }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is List<Genre> genres)
            {
                var combinedItems = new List<CombinedItem>();
                for (int i = 0; i < genres.Count; i++)
                {
                    combinedItems.Add(new CombinedItem
                    {
                        GenreName = genres[i].GenreName,
                        Icon = (i < genres.Count - 1) ? "\uF83F" : string.Empty
                    });
                }
                return combinedItems;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class TotalMovieConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is ObservableCollection<MovieCommand> movies)
            {
                int total = 0;
                total = movies.Count;
                return total;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    



}
