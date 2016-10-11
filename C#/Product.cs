using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQTest
{
    public class Product
    {
        public Product()
        {
        }

        public Product(int productID, String productName, String category,
            double unitPrice, int unitsInStock)
        {
            this.ProductID = productID;
            this.ProductName = productName;
            this.Category = category;
            this.UnitPrice = unitPrice;
            this.UnitsInStock = unitsInStock;
        }

        public int ProductID;
        public String ProductName;
        public String Category;
        public double UnitPrice;
        public int UnitsInStock;

        public override String ToString()
        {
            return "Product[productID=" + ProductID + ", ProductName=" + ProductName + ", Category=" + Category +
                   ", UnitPrice=" + UnitPrice + ", UnitsInStock=" + UnitsInStock + "]";
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime*result
                     + ((Category == null) ? 0 : Category.GetHashCode());
            result = prime*result + ProductID;
            result = prime*result
                     + ((ProductName == null) ? 0 : ProductName.GetHashCode());
            long temp;
            temp = BitConverter.DoubleToInt64Bits(UnitPrice);
            result = prime*result + (int) (temp ^ (int) ((uint) temp >> 32));
            result = prime*result + UnitsInStock;
            return result;
        }

        public override bool Equals(Object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            Product other = (Product) obj;
            if (Category == null)
            {
                if (other.Category != null)
                    return false;
            }
            else if (!Category.Equals(other.Category))
                return false;
            if (ProductID != other.ProductID)
                return false;
            if (ProductName == null)
            {
                if (other.ProductName != null)
                    return false;
            }
            else if (!ProductName.Equals(other.ProductName))
                return false;
            if (BitConverter.DoubleToInt64Bits(UnitPrice) != BitConverter.DoubleToInt64Bits(other.UnitPrice))
                return false;
            if (UnitsInStock != other.UnitsInStock)
                return false;
            return true;
        }
    }
}
