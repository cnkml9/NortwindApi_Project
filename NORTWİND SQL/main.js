// main.js dosyası

document.addEventListener("DOMContentLoaded", () => {
    const selectElement = document.getElementById("floatingSelect");
    const inputContainer = document.getElementById("inputContainer");
    const inputValue = document.getElementById("inputValue");
    const showBtn = document.getElementById("showBtn");
    const resultDiv = document.getElementById("result");
    var dolubos = 0;
    selectElement.addEventListener("change", () => {
        const selectedValue = selectElement.value;
        
        if (selectedValue === "1" || selectedValue === "2" || selectedValue === "3") {
            inputContainer.style.display = "block";
           
        } else {
            inputContainer.style.display = "none";
        }
        
    });
    
    showBtn.addEventListener("click", async () => {
        const selectedValue = selectElement.value;
        const valueToSend = inputValue.value;
    
        let endpoint = "";
        
        if (selectedValue === "1") {
            endpoint = `https://localhost:7178/MakeRaise/`+ valueToSend;
        } else if (selectedValue === "2") {
            endpoint = `https://localhost:7178/UpdatePrice/`+  + valueToSend ;
        } else if (selectedValue === "3") {
            endpoint = `https://localhost:7178/ReportFreight`;
        } else if (selectedValue === "4") {
            endpoint = `https://localhost:7178/TopSelling3`;
        } else if (selectedValue === "5") {
            endpoint = `https://localhost:7178/GetBeveragesCategories`;
        } else if (selectedValue === "6") {
            endpoint = `https://localhost:7178/TotalRevenues1917`;
        } else if (selectedValue === "7") {
            endpoint = `https://localhost:7178/GetTop3Suppliers`;
        } else if (selectedValue === "8") {
            endpoint = `https://localhost:7178/ShipperOrderCounts`;
        } else if (selectedValue === "9") {
            endpoint = `https://localhost:7178/CustomerOrdersLeast15`;
        } else if (selectedValue === "11") {
            endpoint = `https://localhost:7178/ShippingWithFederal`;
        } else if (selectedValue === "12") {
            endpoint = `https://localhost:7178/Steven97Report  `;
        } else if (selectedValue === "13") {
            endpoint = `https://localhost:7178/speedy`;
        } else if (selectedValue === "14") {
            endpoint = `https://localhost:7178/GetGermanyCustomer`;
        } else if (selectedValue === "15") {
            endpoint = `https://localhost:7178/Seafood`;
        } else if (selectedValue === "16") {
            endpoint = `https://localhost:7178/SpeedyExpWithNancy`;
        } else if (selectedValue === "17") {
            endpoint = `https://localhost:7178/Eastren`;
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
            endpoint = `https://localhost:7178/BesSellerProduct`;
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




        
        console.log(endpoint);
  
        
        try {
            const response = await fetch(endpoint, {
                method: selectedValue === "1" || selectedValue === "2" || selectedValue === "3" ? "POST" : "GET",
               
            });
            console.log('try çalışıyor');
            if (response.ok) {
                
                const responseData = await response.json();
                resultDiv.style.display = "block";
                resultDiv.innerHTML = "Başarılı!";

                 // Assuming responseData is an array of JSON objects
                 const table = createTable(responseData);
                 document.querySelector(".icerik").innerHTML = table;
            } else {
                
                resultDiv.style.display = "block";
                resultDiv.innerHTML = "Başarısız!";
               
            }
        } catch (error) {

            resultDiv.style.display = "block";
            resultDiv.innerHTML = "Hata oluştu lütfen daha sonra tekrar deneyiniz :";
            console.log(error);
        }

    });
    
});
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
    return `<tr>${keys.map(key => `<td>${item[key]}</td>`).join("")}</tr>`;
}