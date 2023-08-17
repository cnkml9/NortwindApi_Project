document.addEventListener("DOMContentLoaded", () => {
    const selectElement = document.getElementById("floatingSelect");
    const inputContainer = document.getElementById("inputContainer");
    const inputContainer2 = document.getElementById("inputContainer2");
    const inputContainer3 = document.getElementById("inputContainer3");
    const inputValue = document.getElementById("inputValue");
    const inputValue2 = document.getElementById("inputValue2");
    const showBtn = document.getElementById("showBtn");
    const resultDiv = document.getElementById("result");
    const inputValueName = document.getElementById("inputValueName");
    
    const shipName = document.getElementById("shipName");
    const inputYear = document.getElementById("inputYear");
    const MinShippingCost = document.getElementById("MinShippingCost");

    selectElement.addEventListener("change", () => {
        const selectedValue = selectElement.value;

        document.querySelector(".icerik").innerHTML = "";
        document.getElementById("result").innerHTML = "";

        if (selectedValue === "1") {
            inputContainer.style.display = "block";
            inputContainer2.style.display = "none";
            inputContainer3.style.display = "none";
        }
        else if (selectedValue === "2") {
            inputContainer2.style.display = "block";
            inputContainer.style.display = "none";
            inputContainer3.style.display = "none";
        }
        else if (selectedValue === "3") {
            inputContainer3.style.display = "block";
            inputContainer2.style.display = "none";
            inputContainer.style.display = "none";
        } else {
            inputContainer.style.display = "none";
            inputContainer2.style.display = "none";
            inputContainer3.style.display = "none";
        }   
    });

    showBtn.addEventListener("click", async () => {
        const selectedValue = selectElement.value;
        const valueToSend = inputValue.value;
        const valueToSend2 = inputValue2.value;
        const inputValueNameValue = inputValueName.value;
        const shipNameValue = shipName.value;
        const inputYearValue = inputYear.value;
        const MinShippingCostValue = MinShippingCost.value;



        if (selectedValue === "1") {
            await handlePostRequest(selectedValue, valueToSend, resultDiv);
        }
        else if (selectedValue === "2") {
            await handlePostRequest2(selectedValue, valueToSend2, inputValueNameValue, resultDiv);
        }
        else if (selectedValue === "3") {
            await handlePostRequest3(shipNameValue, MinShippingCostValue, inputYearValue, resultDiv);
        }
        else {
            await handleGetRequest(selectedValue , resultDiv);
        } 
         
    });
});

   async function handlePostRequest(selectedValue, valueToSend, resultDiv) {
    let endpoint = "";

    if (selectedValue === "1") {
        if (isNaN(valueToSend)) {
            resultDiv.style.display = "block";
            resultDiv.innerHTML = "Hata: Lütfen bir sayı girin.";
            return;
        }
        endpoint = `https://localhost:7178/MakeRaise/${valueToSend}`;
    }

    try {
        const postData = {
          title: 'Yeni Bir Post',
          content: 'Bu yeni bir gönderidir.'
        };
      
        const response = await fetch(endpoint, {
          method: "POST",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify(postData)    
        });
      
        if (response.ok) {
            const responseData = await response.json();

            resultDiv.style.display = "block";
            resultDiv.innerHTML = "İşlem sonucu: Ürün Bilgileri<br>";

            responseData.forEach(product => {
                resultDiv.innerHTML +=
                    `Ürün Adı: ${product.productName}<br>` +
                    `Eski Fiyat: ${product.unitPrice + product.unitPrice * (valueToSend / 100)}<br>` +
                    `Yeni Fiyat: ${product.unitPrice}<br><br>`;
            });
        }
      } catch (error) {
        resultDiv.style.display = "block";
        // resultDiv.innerHTML = "Hata oluştu: " + error.message;
        console.log("Hata oluştu: ", error);
      }

}

async function handlePostRequest2(selectedValue, inputValueNameValue, inputValue2, resultDiv) {
    let endpoint = "";

    if (selectedValue === "2") {  
        endpoint = `https://localhost:7178/UpdatePrice/${inputValue2}/${inputValueNameValue}`;
    }

    try {
        const postData = {
          title: 'Yeni Bir Post',
          content: 'Bu yeni bir gönderidir.'
        };
      
        const response = await fetch(endpoint, {
          method: "POST",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify(postData)    
        });
      
        if (response.ok) {
            const responseData = await response.json();
            const productInfo = responseData; // API yanıtını ürün bilgileri olarak alın

            // Ürün bilgilerini düzenleyerek "resultDiv" içeriğini güncelle
            resultDiv.style.display = "block";
            resultDiv.innerHTML = "İşlem sonucu: Ürün Bilgileri" +
                `<br>Ürün Adı: ${productInfo.productName}` +
                `<br>Yeni Fiyat: ${productInfo.unitPrice}` +
                `<br>Eski Fiyat: ${productInfo.responseData}`; //hatalı, düzeltilmesi gerek

        }
      } catch (error) {
        resultDiv.style.display = "block";
        // resultDiv.innerHTML = "Hata oluştu: " + error.message;
        console.log("Hata oluştu: ", error);
      }

}

async function handlePostRequest3(shipName, minShippingCost, orderYear, resultDiv) {
    let endpoint = `https://localhost:7178/ReportFreight`;

    try {
        const postData = {
            shipment: shipName,
            minShippingCost: minShippingCost,
            orderYear: orderYear
        };

        const response = await fetch(endpoint, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(postData)
        });

        if (response.ok) {
            const responseData = await response.json();
            resultDiv.style.display = "block";
            resultDiv.innerHTML = "Başarılı!";

            const table = createTable(responseData);
            document.querySelector(".icerik").innerHTML = table;
        } else {
            resultDiv.style.display = "block";
            resultDiv.innerHTML = "Başarısız!";
        }
    } catch (error) {
        resultDiv.style.display = "block";
        resultDiv.innerHTML = "Hata oluştu. Lütfen daha sonra tekrar deneyiniz.";
        console.log("Hata oluştu: ", error);
    }
}


   

      

async function handleGetRequest(selectedValue, resultDiv) {
    let endpoint = "";

    if (selectedValue === "4") {
        endpoint = `https://localhost:7178/TopSelling3`;
    } else if (selectedValue === "5") {
        endpoint = `https://localhost:7178/GetBeveragesCategories`;
    } else if (selectedValue === "6") {
        endpoint = `https://localhost:7178/TotalRevenues1997`;
    } else if (selectedValue === "7") {
        endpoint = `https://localhost:7178/GetTop3Suppliers`;
    } else if (selectedValue === "8") {
        endpoint = `https://localhost:7178/ShipperOrderCounts`;
    } else if (selectedValue === "9") {
        endpoint = `https://localhost:7178/CustomerOrdersLeast15`;
    } else if (selectedValue === "11") {
        endpoint = `https://localhost:7178/ShippingWithFederal`;
    } else if (selectedValue === "12") {
        endpoint = `https://localhost:7178/Steven97Report`;
    } else if (selectedValue === "13") {
        endpoint = `https://localhost:7178/SpeedyOrderNuncyAlfki`;
    } else if (selectedValue === "14") {
        endpoint = `https://localhost:7178/GetGermanyCustomer`;
    } else if (selectedValue === "15") {
        endpoint = `https://localhost:7178/Seafood`;
    } else if (selectedValue === "16") {
        endpoint = `https://localhost:7178/SpeedyExpWithNancy`;
    } else if (selectedValue === "17") {
        endpoint = `https://localhost:7178/Eastern`;
    } else if (selectedValue === "18") {
        endpoint = `https://localhost:7178/ShippersLondon`;
    } else if (selectedValue === "19") {
        endpoint = `https://localhost:7178/DiscontinuedSale`;
    } else if (selectedValue === "20") {
        endpoint = `https://localhost:7178/NewYorkManager`;
    } else if (selectedValue === "21") {
        endpoint = `https://localhost:7178/OrderLast1998`;
    } else if (selectedValue === "22") {
        endpoint = `https://localhost:7178/Tacoma`;
    } else if (selectedValue === "23") {
        endpoint = `https://localhost:7178/BestSellerProduct`;
    } else if (selectedValue === "24") {
        endpoint = `https://localhost:7178/ListBestOrders`;
    } else if (selectedValue === "25") {
        endpoint = `https://localhost:7178/NancysProducts`;
    } else if (selectedValue === "26") {
        endpoint = `https://localhost:7178/HighAverageSales`;
    } else if (selectedValue === "27") {
        endpoint = `https://localhost:7178/USASuplliersBeverages`;
    } else if (selectedValue === "28") {
        endpoint = `https://localhost:7178/ProductTypeCount`;
    }

    try {
        const response = await fetch(endpoint, {
            method: "GET",
            // ... Gerekli ayarlamalar ...
        });

        if (response.ok) {
            const responseData = await response.json();
            resultDiv.style.display = "block";
            resultDiv.innerHTML = "Başarılı!";

            const table = createTable(responseData);
            document.querySelector(".icerik").innerHTML = table;
        } else {
            resultDiv.style.display = "block";
            resultDiv.innerHTML = "Başarısız!";
        }
    } catch (error) {
        resultDiv.style.display = "block";
        resultDiv.innerHTML = "Hata oluştu. Lütfen daha sonra tekrar deneyiniz.";
        console.log(error);
    }
}

function createTable(data) {
    if (!Array.isArray(data) || data.length === 0) {
        return "<p>No data available</p>";
    }

    const keys = Object.keys(data[0]);

    const tableHeader = `<thead><tr>${keys.map(key => `<th>${key}</th>`).join("")}</tr></thead>`;

    const tableBody = `<tbody>${data.map(item => createTableRow(item, keys)).join("")}</tbody>`;

    return `<table class="table">${tableHeader}${tableBody}</table>`;
}

function createTableRow(item, keys) {
    return `<tr>${keys.map(key => {
        const value = item[key];
        if (typeof value === 'object' && value !== null) {
            return `<td>${getObjectValues(value).map(val => `${val[0]}: ${val[1]}`).join("<br>")}</td>`;
        } else {
            return `<td>${value}</td>`;
        }
    }).join("")}</tr>`;
}

function getObjectValues(obj) {
    return Object.entries(obj).map(([key, value]) => {
        if (typeof value === 'object' && value !== null) {
            return [key, getObjectValues(value).map(val => `${key}.${val[0]}: ${val[1]}`)];
        } else {
            return [key, value];
        }
    });
}
