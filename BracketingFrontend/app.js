document.addEventListener('DOMContentLoaded', () => {
    fetchPairs();
/*    displayPairs();
    addParticipant();*/
});

function fetchPairs() {
    fetch('http://localhost:5089/api/Pairs')
        .then(response => response.json())
        .then(data => {
            console.log(data);
            displayPairs(data);
        })
        .catch(error => console.error('Error fetching pairs:', error));
}

function displayPairs(pairs) {
    const contentDiv = document.getElementById('pairs');
    if (!contentDiv) {
        console.error('Element with id "pairs" not found.');
        return; // Exit function if the element is not found
    }
    contentDiv.innerHTML = '';
    pairs.forEach(pair => {
        contentDiv.innerHTML += `<p>${pair.first} ---  ${pair.second}</p>`;
    });
}



function addParticipant() {
    const pName = document.getElementById('participantName').value.trim();

    // Check if the name is not empty
    if (pName === "") {
        console.error('Participant name is empty.');
        return;
    }

    fetch('http://localhost:5089/api/Pairs', {  //let op de hoofdletter in de gare url... 
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(pName) // Sending the name as a raw string // zie andere functie voor json opbouw voor later. 
    })
        .then(response => {
            if (response.ok) {
                document.getElementById('participantName').value = '';
                console.log('Participant added');
                fetchPairs(); // Function to fetch updated pairs list
            } else {
                console.error('Error adding participant:', response.statusText);
            }
        })
        .catch(error => console.error('Error:', error));
}





/*
function addParticipant() {
    const pName = document.getElementById('participantName').value.trim(); 

    const participant = {
        name: pName
    }; 

    fetch('http://localhost:5089/api/pairs', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(participant)
    })
        .then(response => {
            if (response.ok) {
                document.getElementById('participantName').value = '';
                console.log('Participant added');
                fetchPairs();
            } else {
                console.error('Error adding participant:', response.statusText);
            }
        })
        .catch(error => console.error('Error:', error));
}*/