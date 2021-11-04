using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using steimatzky.Data;
using steimatzky.Model;
using Steimatzky.Extensions;
using Steimatzky.Exceptions;

namespace Steimatzky
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProductRepo<BaseProduct> productRepo;
        private List<BaseProduct> baseProducts;
        List<DDL_Genre> objGenersList;
        List<DDL_Genre> objEditGenersList;
        BaseProduct productToEdit;
        private int comboItemTypeIndex;
        //private int comboBoxProductTypeIndex;
        private int comboItemSortMethodIndex;
        private int comboItemEditGenerIndex;
        //private int[] selectedGenres;
        private int quantityToRemove;
        private enum WhichList
        {
            Unknown,
            InStock,
            Removed,
            Sold
        }
        private WhichList whichList;
        private int comboBoxProductTypeIndex;

        public MainWindow()
        {
            this.InitializeComponent();
            productRepo = new ProductRepo<BaseProduct>();
            objGenersList = new List<DDL_Genre>();
            objEditGenersList = new List<DDL_Genre>();
            PrintDate.SelectedDate = DateTime.Today;
            whichList = new WhichList();
            baseProducts = productRepo.GetAll();
        }
        //

        //public MainWindow()
        //{
        //    InitializeComponent();
        //}
        //private void ListBind()
        //{
        //    //List<Imgs> imgs = new List<Imgs>();
        //    //imgs.Add(new Imgs() { ImageSource = @"\images\BooksIcon.jpg" });
        //    //imgs.Add(new Imgs() { ImageSource = @"\images\BooksIcon.jpg" });
        //    //imgs.Add(new Imgs() { ImageSource = @"\images\BooksIcon.jpg" });
        //    //Data.ItemsSource = imgs;
        //}

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Data.Items.Clear();
            if (UsersManager.IsChecked && !UsersWorker.IsChecked && Password.Password == "1")
            {
                GoToManagerDBSelectGrid();
                UsersManagerSearch.IsChecked = true;
                UsersWorkerSearch.IsChecked = false;
            }
            else if (UsersWorker.IsChecked && !UsersManager.IsChecked && Password.Password == "Worker")
            {
                whichList = WhichList.InStock;
                baseProducts = productRepo.GetAll();
                GoToSearchGrid();
                UsersWorkerSearch.IsChecked = true;
                UsersManagerSearch.IsChecked = false;
            }
            else
            {
                MessageBox.Show("Your password is not correnct");
            }
        }

        /// <summary>
        /// ^LoginGrid^
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <summary>
        /// AddingGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void RemovedDB_Click(object sender, RoutedEventArgs e)
        {
            whichList = WhichList.Removed;
            baseProducts = productRepo.GetData(ProductStatus.Remove);
            GoToSearchGrid();
        }

        private void SoldDB_Click(object sender, RoutedEventArgs e)
        {
            whichList = WhichList.Sold;

        }

        private void BindingDB_Click(object sender, RoutedEventArgs e)
        {
            whichList = WhichList.InStock;
            baseProducts = productRepo.GetData(ProductStatus.InStock);
            GoToAddingGrid();
        }

        /// <summary>
        /// ^DBGrid^
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <summary>
        /// AddingGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        ///

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            BaseProduct product;
            double price = 0;
            int quantity = 0;
            DateTime dateTime = DateTime.Now;
            bool isIdLigal = true;
            bool isPriceLigal = true;
            bool isPrintDateLigal = true;
            bool isQuntityLigal = true;
            bool isProductLigal = (isIdLigal && isPriceLigal && isPrintDateLigal && isQuntityLigal);

            try
            {
                price = Price.Text.ToDouble();
                if (price <= 0)
                {
                    isPriceLigal = false;
                    throw new ExceptionIO("Price Can not be less or equle to zero");
                }
            }
            catch (ExceptionIO ex)
            {
                MessageBox.Show($"Invalid price Reason: {ex}");
            }

            try
            {
                quantity = Quntity.Text.ToInt();
                if (quantity <= 0)
                {
                    isQuntityLigal = false;
                    throw new ExceptionIO("Quntity Can not be less or equle to zero");
                }
            }
            catch (ExceptionIO ex)
            {
                MessageBox.Show($"Invalid Quntity Reason: {ex}");
            }

            try
            {
                if (PrintDate.DisplayDate > DateTime.Now)
                {
                    isPrintDateLigal = false;
                    throw new ExceptionIO("Print Date Can not be less then today date");
                }
                else
                {
                    dateTime = PrintDate.DisplayDate;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid Print Date Reason: {ex}");
            }

            if (isProductLigal)
            {
                switch (comboItemTypeIndex)//TBD??? BoolFunction Check
                {
                    case 0:
                        BookGenre[] bookGenres;
                        bookGenres = GetBookGeners(objGenersList);
                        product = new Book(Name.Text, Author.Text, price, dateTime, quantity, steimatzky.Model.ProductType.Book, bookGenres, Summary.Text, Edition.Text);
                        product.AddingDate = DateTime.Now;
                        product.ISBN = Guid.NewGuid();
                        if (productRepo.GetAll().Count == 0) product.Id = 1;
                        else product.Id = ++productRepo.GetAll().Last().Id;
                        productRepo.AddOrRemoveOrUpdate(product, ProductOperation.Add);
                        break;
                    case 1:
                        JournalGenre[] journalGenres;
                        journalGenres = GetJournalGenres(objGenersList);
                        product = new Journal(Name.Text, price, dateTime, quantity, steimatzky.Model.ProductType.Journal, journalGenres);
                        product.AddingDate = DateTime.Now;
                        if (productRepo.GetAll() == null) product.Id = 1;
                        else product.Id = ++productRepo.GetAll().Last().Id;
                        productRepo.AddOrRemoveOrUpdate(product, ProductOperation.Add);
                        break;
                    default:
                        MessageBox.Show("No Item Here");
                        break;
                }
                if (productRepo.AddOrRemoveOrUpdate(null, ProductOperation.Update) == false)
                {
                    MessageBox.Show("Could not read from the Data base");
                }
            }
            else
            {
                MessageBox.Show("your prouduct was not added");
            }
            ResetAddingGrid();
        }

        private void comboBoxProductType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboItemTypeIndex = ProductType.SelectedIndex;
            genreListBox.Items.Clear();
            comboBoxProductTypeIndex = ProductType.SelectedIndex;
            AddElementsInList(ProductType.SelectedIndex, Genre, objGenersList);
            BindGenreDropDown(Genre, objGenersList);
        }

        private void BindGenreDropDown(ComboBox comboBox, List<DDL_Genre> dL_Genres)
        {
            comboBox.ItemsSource = dL_Genres;
        }

        private void Genre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Gener_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Genre.SelectedIndex > 0) Genre.Text = objGenersList.Find(x => (x.Genre_ID == Genre.SelectedIndex)).Genre_Name;//TBD
        }

        private void AllCheckbocx_CheckedAndUnchecked(object sender, RoutedEventArgs e)
        {
            BindListBOX(genreListBox, objGenersList);
        }

        private void BindListBOX(ListBox listBox, List<DDL_Genre> dL_Genres)
        {
            listBox.Items.Clear();
            foreach (var Genre in dL_Genres)
            {
                if (Genre.Check_Status == true)
                {
                    listBox.Items.Add(Genre.Genre_Name);
                }
            }
        }

        private void AddElementsInList(int index, ComboBox comboBox, List<DDL_Genre> dL_Genres)
        {
            comboBox.ItemsSource = null;
            comboBox.Items.Refresh();
            dL_Genres.Clear();

            if (index == 0)
            {
                for (int i = 0; i < Enum.GetNames(typeof(BookGenre)).Length; i++)
                {
                    DDL_Genre genre = new DDL_Genre();
                    genre.Genre_ID = i;
                    genre.Genre_Name = ((BookGenre)i).ToString();
                    dL_Genres.Add(genre);
                }
                Author.IsReadOnly = false;
                Author.Clear();
                Edition.IsReadOnly = false;
                Edition.Clear();
                Summary.IsReadOnly = false;
                Summary.Clear();
            }

            if (index == 1)
            {
                for (int i = 0; i < Enum.GetNames(typeof(JournalGenre)).Length; i++)
                {
                    DDL_Genre genre = new DDL_Genre();
                    genre.Genre_ID = i;
                    genre.Genre_Name = ((JournalGenre)i).ToString();
                    dL_Genres.Add(genre);
                }
                Author.IsReadOnly = true;
                Author.Text = "<Read Only>";
                Edition.IsReadOnly = true;
                Edition.Text = "<Read Only>";
                Summary.IsReadOnly = true;
                Summary.Text = "<Read Only>";
            }
        }

        /// <summary>
        /// ^AddingGrid^
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void GetAll_Click(object sender, RoutedEventArgs e)
        {
            Data.Items.Clear();
            if (whichList == WhichList.InStock)
            {
                productRepo.GetData(ProductStatus.InStock).ForEach(p => Data.Items.Add(p));
            }
            if (whichList == WhichList.Removed)
            {
                productRepo.GetData(ProductStatus.Remove).ForEach(p => Data.Items.Add(p));
            }
            if (whichList == WhichList.Sold)
            {
                //productRepo.GetData(ProductStatus.Sold).ForEach(p => Data.Items.Add(p));
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)//Add Exeptions
        {
            Data.Items.Clear();
            Data.Items.Refresh();
            switch (comboItemTypeIndex)
            {
                case 0://Name
                    if (whichList == WhichList.InStock)
                    {
                        baseProducts = (List<BaseProduct>)productRepo.GetBy((product) => (product.Name.ToUpper() == Search.Text.ToUpper() && product.QuantityInStock > 0));
                    }
                    if (whichList == WhichList.Removed)
                    {
                        baseProducts = (List<BaseProduct>)productRepo.GetBy((product) => (product.Name.ToUpper() == Search.Text.ToUpper() && product.QuantityRemoved > 0));
                    }
                    break;
                case 1://Id
                    int id = 0;
                    try
                    {
                        id = Search.Text.ToInt();
                        if (id <= 0)
                        {
                            throw new ExceptionIO("Search text is iligal");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Id {ex}");
                    }
                    if (whichList == WhichList.InStock)
                    {
                        baseProducts = (List<BaseProduct>)productRepo.GetBy((product) => (product.Id == id) && product.QuantityInStock > 0);
                    }
                    if (whichList == WhichList.Removed)
                    {
                        baseProducts = (List<BaseProduct>)productRepo.GetBy((product) => (product.Id == id) && product.QuantityRemoved > 0);
                    }
                    break;
                case 2://Price
                    double price = 0;
                    try
                    {
                        price = Search.Text.ToDouble();
                        if (price < 0)
                        {
                            throw new InvalidDataException();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Price {ex.Message}");
                    }

                    if (whichList == WhichList.InStock)
                    {
                        baseProducts = (List<BaseProduct>)productRepo.GetBy((product) => (product.ProductPrice == price && product.QuantityInStock > 0));
                    }
                    if (whichList == WhichList.Removed)
                    {
                        baseProducts = (List<BaseProduct>)productRepo.GetBy((product) => (product.ProductPrice == price && product.QuantityRemoved > 0));
                    }
                    break;
                case 3://BooksGeners
                    BookGenre bGenre = BookGenre.Unknown;
                    if (Categorys.SelectedItem != null && Categorys.SelectedItem.ToString() != "All")
                    {
                        bGenre = (BookGenre)Enum.Parse(typeof(BookGenre), Categorys.SelectedItem.ToString(), true);
                    }
                    if (whichList == WhichList.InStock)
                    {
                        baseProducts = (List<BaseProduct>)productRepo.GetBy(
                    (product) =>
                    {
                        if ((Categorys.SelectedItem == null || Categorys.SelectedItem.ToString() == "All") && product.QuantityInStock >= 0)
                        {
                            return (product.ProductType == steimatzky.Model.ProductType.Book);
                        }
                        else if (product.GetType() == typeof(Book))
                        {
                            Book book = (Book)product; ;
                            return (book.BookGenres.Contains(bGenre) && product.QuantityInStock > 0);
                        }
                        return false;
                    });
                    }
                    if (whichList == WhichList.Removed)
                    {
                        baseProducts = (List<BaseProduct>)productRepo.GetBy(
                    (product) =>
                    {
                        if ((Categorys.SelectedItem == null || Categorys.SelectedItem.ToString() == "All") && product.QuantityRemoved > 0)
                        {
                            return (product.ProductType == steimatzky.Model.ProductType.Book);
                        }
                        else if (product.GetType() == typeof(Book))
                        {
                            Book book = (Book)product;
                            return (book.BookGenres.Contains(bGenre) && product.QuantityRemoved > 0);
                        }
                        return false;
                    });
                    }
                    break;
                case 4://JournalGeners
                    JournalGenre jGenre = JournalGenre.Unknown;
                    if (Categorys.SelectedItem != null && Categorys.SelectedItem.ToString() != "All")
                    {
                        jGenre = (JournalGenre)Enum.Parse(typeof(JournalGenre), Categorys.SelectedItem.ToString(), true);
                    }

                    if (whichList == WhichList.InStock)
                    {
                        baseProducts = (List<BaseProduct>)productRepo.GetBy((product) =>
                        {
                            if ((Categorys.SelectedItem == null || Categorys.SelectedItem.ToString() == "All") && product.QuantityInStock >= 0)
                            {
                                return (product.ProductType == steimatzky.Model.ProductType.Journal);
                            }
                            else if (product.GetType() == typeof(Journal) && product.QuantityInStock > 0)
                            {
                                Journal journal = (Journal)product;
                                return (journal.JournalGenres.Contains(jGenre));
                            }
                            return false;
                        });

                    }
                    if (whichList == WhichList.Removed)
                    {
                        baseProducts = (List<BaseProduct>)productRepo.GetBy((product) =>
                        {
                            if ((Categorys.SelectedItem == null || Categorys.SelectedItem.ToString() == "All") && product.QuantityRemoved > 0)
                            {
                                return (product.ProductType == steimatzky.Model.ProductType.Journal);
                            }
                            else if (product.GetType() == typeof(Journal))
                            {
                                Journal journal = (Journal)product;
                                return (journal.JournalGenres.Contains(jGenre) && product.QuantityRemoved > 0);
                            }
                            return false;
                        });

                    }
                    break;
                default:
                    MessageBox.Show("Not corect search method");
                    break;
            }

            if (baseProducts != null)
            {
                baseProducts.ForEach(item => Data.Items.Add(item));
            }
        }

        private void Search_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Search.Text == "Search product...") Search.Text = "";
        }

        private void SerchMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboItemTypeIndex = SearchMethod.SelectedIndex;
            switch (comboItemTypeIndex)
            {
                case 0:
                    Categorys.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    Categorys.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    Categorys.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    CreateCategorysCombo(3);
                    break;
                case 4:
                    CreateCategorysCombo(4);
                    break;
                default:
                    break;
            }
        }

        private void CreateCategorysCombo(int comboItemTypeIndex)
        {
            switch (comboItemTypeIndex)
            {
                case 3:
                    Categorys.Items.Clear();
                    Categorys.Items.Add("All");
                    for (int i = 0; i < Enum.GetNames(typeof(BookGenre)).Length; i++)
                    {
                        string genre;
                        genre = ((BookGenre)i).ToString();
                        Categorys.Items.Add(genre);
                    }
                    Categorys.Visibility = Visibility.Visible;
                    break;
                case 4:
                    Categorys.Items.Clear();
                    Categorys.Items.Add("All");
                    for (int i = 0; i < Enum.GetNames(typeof(JournalGenre)).Length; i++)
                    {
                        string genre;
                        genre = ((JournalGenre)i).ToString();
                        Categorys.Items.Add(genre);
                    }
                    Categorys.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void Asc_Click(object sender, RoutedEventArgs e)
        {
            Data.Items.Clear();
            Data.Items.Refresh();
            switch (comboItemSortMethodIndex)
            {
                case 0:
                    baseProducts.Sort((p1, p2) => (p1.Name.CompareTo(p2.Name)));
                    break;
                case 1:
                    baseProducts.Sort((p1, p2) => (p1.Id.CompareTo(p2.Id)));
                    break;
                case 2:
                    baseProducts.Sort((p1, p2) => (p1.ProductPrice.CompareTo(p2.ProductPrice)));
                    break;
                default:
                    break;
            }
            baseProducts.ForEach(p => Data.Items.Add(p));
        }

        private void Desc_Click(object sender, RoutedEventArgs e)
        {
            switch (comboItemSortMethodIndex)
            {
                case 0:
                    baseProducts.Sort((p1, p2) => (p2.Name.CompareTo(p1.Name)));
                    Data.Items.Clear();
                    break;
                case 1:
                    baseProducts.Sort((p1, p2) => (p2.Id.CompareTo(p1.Id)));
                    Data.Items.Clear();
                    break;
                case 2:
                    baseProducts.Sort((p1, p2) => (p2.ProductPrice.CompareTo(p1.ProductPrice)));
                    Data.Items.Clear();
                    break;
                default:
                    break;
            }
            baseProducts.ForEach(p => Data.Items.Add(p));
        }

        private void SortMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            comboItemSortMethodIndex = combo.SelectedIndex;
        }

        private void BackToAddingGrid_Click(object sender, RoutedEventArgs e)
        {
            GoToAddingGrid();
        }

        private void GoToSearchGrid_Click(object sender, RoutedEventArgs e)
        {
            editGenre.ItemsSource = null;
            editGenre.Items.Refresh();
            GenreEditListBox.ItemsSource = null;
            GenreEditListBox.Items.Refresh();
            GoToSearchGrid();
        }

        private void QuantityToRemove_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            quantityToRemove = textBox.Text.ToInt();
        }

        private void DeleteQuntity_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            BaseProduct product = b.CommandParameter as BaseProduct;

            if (MessageBox.Show($"Are You Sure you want to remove {quantityToRemove} from {product.Name}?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                MessageBox.Show("The quntity did not changed");
            }
            else
            {
                if (whichList == WhichList.InStock)
                {
                    if (quantityToRemove > product.QuantityInStock)
                    {
                        MessageBox.Show($"Cant Delete more then the quantity in stock: {product.QuantityInStock}");
                    }
                    else
                    {
                        productRepo.AddOrRemoveOrUpdate(product, ProductOperation.Remove, quantityToRemove);
                        if (product.QuantityInStock == 0)
                        {
                            MessageBox.Show($"Deleted: {product}");
                        }
                        else
                        {
                            MessageBox.Show($"New quantity in stock: {product.QuantityInStock}");
                        }
                    }
                }

                if (whichList == WhichList.Removed)
                {
                    MessageBox.Show($"This Button is invalid in this state(RemovedList)");
                }

                if (whichList == WhichList.Sold)
                {
                    MessageBox.Show($"This Button is invalid in this state(SoldList)");
                }
                Data.Items.Refresh();
            }
        }

        private void DeleteProductClick(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            BaseProduct product = b.CommandParameter as BaseProduct;
            if (MessageBox.Show($"Are You Sure you want to remove {product.Name}?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                MessageBox.Show("The quntity did not changed");
            }
            else
            {
                productRepo.AddOrRemoveOrUpdate(product, ProductOperation.Remove, product.QuantityInStock);

            }
        }

        private void Data_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (whichList == WhichList.InStock && Data.SelectedItem != null)
            {
                editGenre.ItemsSource = null;
                editGenre.Items.Refresh();
                GenreEditListBox.Items.Clear();
                objEditGenersList.Clear();
                GoToEditGrid();
                SetEditGrid();
                BindGenreDropDown(editGenre, objEditGenersList);
            }
        }

        private void SetEditGrid()
        {
            productToEdit = (BaseProduct)Data.SelectedItem;
            EditName.Text = productToEdit.Name;
            EditPrintDate.Text = productToEdit.PrintDate.ToString();
            EditQuntity.Text = productToEdit.QuantityInStock.ToString();
            EditPrice.Text = productToEdit.ProductPrice.ToString();
            EditId.Text = productToEdit.Id.ToString();
            if (productToEdit.GetType() == typeof(Book))
            {
                Book book = (Book)Data.SelectedItem;
                EditAuthor.Text = book.Author.ToString();
                EditEdition.Text = book.Edition;
                EditSummary.Text = book.Summary;
                EditProductType.Text = steimatzky.Model.ProductType.Book.ToString();
                AddElementsInList(0, editGenre, objEditGenersList);
                int bookCounter = 0;
                foreach (var item in objEditGenersList)
                {
                    if (bookCounter < book.BookGenres.Length && (int)book.BookGenres[bookCounter] == item.Genre_ID)
                    {
                        item.Check_Status = true;
                        GenreEditListBox.Items.Add(item.Genre_Name);
                        bookCounter++;
                    }
                }
            }

            if (productToEdit.GetType() == typeof(Journal))
            {
                Journal journal = (Journal)Data.SelectedItem;
                EditProductType.Text = steimatzky.Model.ProductType.Journal.ToString();
                AddElementsInList(1, editGenre, objEditGenersList);
                int journalCounter = 0;
                foreach (var item in objEditGenersList)
                {
                    if (journalCounter < journal.JournalGenres.Length && (int)journal.JournalGenres[journalCounter] == item.Genre_ID)
                    {
                        item.Check_Status = true;
                        GenreEditListBox.Items.Add(item.Genre_Name);
                        journalCounter++;
                    }
                }
            }
        }

        /// <summary>
        /// ^searchGrid^
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        private void EditAllCheckbocx_CheckedAndUnchecked(object sender, RoutedEventArgs e)
        {
            BindListBOX(GenreEditListBox, objEditGenersList);
        }

        private void ComboBoxGenreEdit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboItemEditGenerIndex = editGenre.SelectedIndex;
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (whichList == WhichList.InStock)
            {
                DateTime AddingDate = productToEdit.AddingDate;
                double price = 0;
                int quntity = 0;
                DateTime dateTime = DateTime.Now;
                bool isPriceLigal = true;
                bool isQuntityLigal = true;
                bool isPrintDateLigal = true;
                bool isEditLigal = isPriceLigal && isPrintDateLigal && isQuntityLigal;

                try
                {
                    price = Price.Text.ToDouble();
                    if (price <= 0)
                    {
                        isPriceLigal = false;
                        throw new ExceptionIO("Price Can not be less or equle to zero");
                    }
                }
                catch (ExceptionIO ex)
                {
                    MessageBox.Show($"Invalid price Reason: {ex}");
                }

                try
                {
                    quntity = EditQuntity.Text.ToInt();
                    if (quntity <= 0)
                    {
                        isQuntityLigal = false;
                        throw new InvalidDataException();
                    }
                }
                catch (ExceptionIO ex)
                {
                    MessageBox.Show($"Invalid quntity Reason: {ex}");
                }

                try
                {
                    if ((DateTime)EditPrintDate.SelectedDate > DateTime.Now)
                    {
                        isPrintDateLigal = false;
                        throw new ExceptionIO("Date time Can not be less the today time");
                    }
                    else
                    {
                        dateTime = (DateTime)EditPrintDate.SelectedDate;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Invalid print date Reason: {ex.Message}");
                }

                if (isEditLigal)
                {
                    if (productToEdit.GetType() == typeof(Book))
                    {
                        Book book = productToEdit as Book;
                        BookGenre[] bookGenres = new BookGenre[(int)BookGenre.TrueCrime];
                        bookGenres = GetBookGeners(objEditGenersList);
                        book.BookGenres = bookGenres;
                        book.Name = EditName.Text;
                        book.PrintDate = dateTime;
                        book.ProductPrice = price;
                        book.QuantityInStock = quntity;
                        book.Edition = EditEdition.Text;
                        book.Author = EditAuthor.Text;
                    }
                    if (productToEdit.GetType() == typeof(Journal))
                    {
                        Journal journal = productToEdit as Journal;
                        JournalGenre[] journalGenres = new JournalGenre[(int)JournalGenre.Essays];
                        journalGenres = GetJournalGenres(objEditGenersList);
                        journal.JournalGenres = journalGenres;
                        JournalGenre bGenre = (JournalGenre)comboItemEditGenerIndex;
                        journal.JournalGenres.Append(bGenre);
                        journal.Name = EditName.Text;
                        journal.PrintDate = dateTime;
                        journal.ProductPrice = price;
                        journal.QuantityInStock = quntity;
                    }
                    productRepo.AddOrRemoveOrUpdate(productToEdit, ProductOperation.Update);
                }
            }
        }

        private void EditGener_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void EditGenre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// ^EditGrid^
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void GoToSearchGrid()
        {
            if (UsersManagerSearch.IsChecked && whichList == WhichList.InStock) BackToAddingGrid.Visibility = Visibility.Visible;
            else BackToAddingGrid.Visibility = Visibility.Collapsed;
            EditGrid.Visibility = Visibility.Collapsed;
            ManagerDBSelect.Visibility = Visibility.Collapsed;
            LoginGrid.Visibility = Visibility.Collapsed;
            AddingGrid.Visibility = Visibility.Collapsed;
            SearchGrid.Visibility = Visibility.Visible;
        }

        private void GoToAddingGrid()
        {
            EditGrid.Visibility = Visibility.Collapsed;
            ManagerDBSelect.Visibility = Visibility.Collapsed;
            LoginGrid.Visibility = Visibility.Collapsed;
            SearchGrid.Visibility = Visibility.Collapsed;
            if (productRepo.GetAll().Count != 0) Id.Text = (++productRepo.GetAll().Last().Id).ToString();
            else Id.Text = "1";
            AddingGrid.Visibility = Visibility.Visible;
        }

        private void GoToLoginGrid()
        {
            EditGrid.Visibility = Visibility.Collapsed;
            ManagerDBSelect.Visibility = Visibility.Collapsed;
            SearchGrid.Visibility = Visibility.Collapsed;
            AddingGrid.Visibility = Visibility.Collapsed;
            LoginGrid.Visibility = Visibility.Visible;
        }

        private void GoToManagerDBSelectGrid()
        {
            EditGrid.Visibility = Visibility.Collapsed;
            SearchGrid.Visibility = Visibility.Collapsed;
            AddingGrid.Visibility = Visibility.Collapsed;
            LoginGrid.Visibility = Visibility.Collapsed;
            ManagerDBSelect.Visibility = Visibility.Visible;
        }

        private void GoToEditGrid()
        {
            SearchGrid.Visibility = Visibility.Collapsed;
            AddingGrid.Visibility = Visibility.Collapsed;
            LoginGrid.Visibility = Visibility.Collapsed;
            ManagerDBSelect.Visibility = Visibility.Collapsed;
            EditGrid.Visibility = Visibility.Visible;
        }

        private void ResetAddingGrid()
        {
            Name.Clear();
            Author.Clear();
            Quntity.Clear();
            Price.Clear();
            Id.Text = (++productRepo.GetAll().Last().Id).ToString();
            PrintDate.DisplayDate = DateTime.Now;
            PrintDate.Text = DateTime.Now.ToString();
            Edition.Clear();
            Summary.Clear();
            Genre.Text = "Please Select Type";
            ProductType.Text = "Please Select Genres";
        }

        private void GoToLoginGrid_Click(object sender, RoutedEventArgs e)
        {
            Password.Password = "";
            GoToLoginGrid();
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuHelp_Click(object sender, RoutedEventArgs e)
        {
            if (LoginGrid.Visibility == Visibility.Visible)
            {
                MessageBox.Show("Enter your password and press the green button\n" +
                                "with the Login text");
            }
        }

        private void SellProduct_Click(object sender, RoutedEventArgs e)
        {

        }

        private BookGenre[] GetBookGeners(List<DDL_Genre> dDL_s)
        {
            BookGenre[] bookGenres = new BookGenre[(int)BookGenre.TrueCrime];
            int bookCounter = 0;
            foreach (var item in dDL_s)
            {
                if (item.Check_Status == true)
                {
                    BookGenre bookGenre;
                    Enum.TryParse(item.Genre_Name, out bookGenre);
                    bookGenres[bookCounter] = bookGenre;
                    bookCounter++;
                }
            }
            return bookGenres;
        }

        private JournalGenre[] GetJournalGenres(List<DDL_Genre> dDL_s)
        {
            JournalGenre[] journalGenres = new JournalGenre[(int)JournalGenre.Essays];
            int journalCounter = 0;
            foreach (var item in dDL_s)
            {
                if (item.Check_Status == true)
                {
                    JournalGenre journalGenre;
                    Enum.TryParse(item.Genre_Name, out journalGenre);
                    journalGenres[journalCounter] = journalGenre;
                    journalCounter++;
                }
            }
            return journalGenres;
        }

        /// <summary>
        /// ^General^
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

    }
    public class Imgs
    {
        public string ImageSource { get; set; }
    }

    public class DDL_Genre
    {
        public int Genre_ID
        {
            get;
            set;
        }
        public string Genre_Name
        {
            get;
            set;
        }
        public Boolean Check_Status
        {
            get;
            set;
        }
    }
}

