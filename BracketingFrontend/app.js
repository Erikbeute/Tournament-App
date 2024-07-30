/*document.addEventListener('DOMContentLoaded', () => {
    fetchPairs();
});

function fetchPairs() {
    fetch('http://localhost:5089/api/Pairs')
        .then(response => response.json())
        .then(data => {
            console.log('Fetched pairs:', data);
            displayPersons(data);
        })
        .catch(error => console.error('Error fetching pairs:', error));
}
function displayPersons(pairs) {
    const contentDiv = document.getElementById('pairs');
    if (!contentDiv) {
        console.error('Element with id "pairs" not found.');
        return;
    }
    contentDiv.innerHTML = '';
    contentDiv.innerHTML += `<p>Gekregen array: ${JSON.stringify(pairs)}</p>`;

    pairs.forEach((pair, index) => {
        contentDiv.innerHTML += `<p>Deelnemer:  ${index + 1}: ${JSON.stringify(pair)}</p>`;
    });
}


function addParticipant() {
    const pName = document.getElementById('participantName').value.trim();

    if (pName === "") {
        console.error('Participant name is empty.');
        return;
    }

    fetch('http://localhost:5089/api/Pairs', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(pName)
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
}

function makeRandomPairs() {
    // Fetch the list of participants
    fetch('http://localhost:5089/api/Pairs')
        .then(response => response.json())
        .then(data => {
            // Shuffle the list of participants
            const shuffledParticipants = data.sort(() => Math.random() - 0.5);

            // Create pairs from the shuffled list
            const pairs = [];
            for (let i = 0; i < shuffledParticipants.length; i += 2) {
                if (i + 1 < shuffledParticipants.length) {
                    pairs.push([shuffledParticipants[i], shuffledParticipants[i + 1]]);
                } else {
                    // If there's an odd number of participants, the last one is left out or moved to the next round
                    pairs.push([shuffledParticipants[i]]);
                }
            }
            // Display the pairs
            displayPairs(pairs);
            // Save the pairs
            postPairs(pairs);
        })
   .catch(error => console.error('Error fetching participants:', error));
}

// this is a function, but is called from the makeRandomPairs function. 
function postPairs(pairs) {
    fetch('http://localhost:5089/api/pairs/savepairs', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(pairs)
    })
        .then(response => {
            if (response.ok) {
                console.log('Pairs saved successfully');
            } else {
                console.error('Error saving pairs:', response.statusText);
            }
        })
        .catch(error => console.error('Error:', error));
}





function displayPairs(pairs) {
    const contentDiv = document.getElementById('pairs');
    if (!contentDiv) {
        console.error('Element with id "pairs" not found.');
        return;
    }
    contentDiv.innerHTML = '';

    pairs.forEach((pair, index) => {
        if (pair.length === 2) {
            contentDiv.innerHTML += `<p>Pair ${index + 1}: ${pair[0]} vs ${pair[1]}</p>`;
        } else {
            contentDiv.innerHTML += `<p>Participant ${pair[0]} has no pair, automatically advances to the next round.</p>`;
        }
    });
}

*/








const apiBase = 'http://localhost:5089/api/Pairs';

document.addEventListener('DOMContentLoaded', () => {
    fetchParticipants();
    fetchPairs();
    fetchResults();
});

function fetchParticipants() {
    fetch(apiBase)
        .then(response => response.json())
        .then(data => {
            const participantsContainer = document.getElementById('participants');
            participantsContainer.innerHTML = data.map(p => `<p>${p}</p>`).join('');
        })
        .catch(error => console.error('Error fetching participants:', error));
}

function addParticipant() {
    const newParticipant = document.getElementById('newParticipant').value;
    if (!newParticipant) return;

    fetch(apiBase, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(newParticipant)
    })
        .then(response => {
            if (response.ok) {
                fetchParticipants();
                document.getElementById('newParticipant').value = '';
            }
        })
        .catch(error => console.error('Error adding participant:', error));
}

function generateRandomPairs() {
    fetch(`${apiBase}/getrandompairs`)
        .then(response => response.json())
        .then(data => {
            displayPairs(data);
        })
        .catch(error => console.error('Error generating random pairs:', error));
}

function displayPairs(pairs) {
    const pairsContainer = document.getElementById('pairs');
    pairsContainer.innerHTML = pairs.map(pair => `
        <div>
            <p>${pair.first} - ${pair.second || 'No opponent'}</p>
            <label>Winner: 
                <select onchange="updateResult('${pair.first}', '${pair.second || ''}', this.value)">
                    <option value="">Select winner</option>
                    <option value="${pair.first}">${pair.first}</option>
                    ${pair.second ? `<option value="${pair.second}">${pair.second}</option>` : ''}
                </select>
            </label>
        </div>
    `).join('');
}

function updateResult(first, second, winner) {
    const loser = (first === winner) ? second : first;

    fetch(`${apiBase}/updatepairresult`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ first, second, winner, loser })
    })
        .then(response => {
            if (response.ok) {
                fetchResults();
            }
        })
        .catch(error => console.error('Error updating result:', error));
}

function fetchResults() {
    fetch(`${apiBase}/getpairswithresults`)
        .then(response => response.json())
        .then(data => {
            const resultsContainer = document.getElementById('results');
            resultsContainer.innerHTML = data.map(result => `
                <div>
                    <p>${result.first} vs ${result.second || 'No opponent'} - Winner: ${result.winner || 'N/A'}</p>
                </div>
            `).join('');
        })
        .catch(error => console.error('Error fetching results:', error));
}

function nextRound() {
    fetch(`${apiBase}/nextround`, {
        method: 'POST'
    })
        .then(response => response.json())
        .then(data => {
            displayPairs(data);
        })
        .catch(error => console.error('Error proceeding to next round:', error));
}
