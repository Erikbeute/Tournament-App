const apiBase = 'http://localhost:5089/api/Pairs';

document.addEventListener('DOMContentLoaded', () => {
    fetchParticipants();
    fetchResults();
});

function fetchParticipants() {
    fetch(apiBase)
        .then(response => response.json())
        .then(data => {
            const participantsContainer = document.getElementById('participants');
            if (data.length === 0) {
                participantsContainer.innerHTML = '<p>No participants.</p>';
            } else {
                participantsContainer.innerHTML = data.map(p => `<p>${p}</p>`).join('');
            }
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
            document.getElementById('participants-section').style.display = 'none';
            document.getElementById('result-section').style.display = 'block';
        })
        .catch(error => console.error('Error generating random pairs:', error));
}

function displayPairs(pairs) {
    const pairsContainer = document.getElementById('pairs');
    pairsContainer.innerHTML = pairs.map(pair => `
        <div>
            <p>${pair.first} - ${pair.second || 'No opponent'}</p>
            <label>Winner: 
                <select id="selectBox" onchange="updateResult('${pair.first}', '${pair.second || ''}', this.value)">
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
            } else {
                return response.json().then(errorData => {
                    console.error('Error updating result:', errorData);
                });
            }
        })
        .catch(error => console.error('Error updating result:', error));
}

function fetchResults() {
    fetch(`${apiBase}/getpairswithresults`)
        .then(response => response.json())
        .then(data => {
            const resultsContainer = document.getElementById('results');

            //when the last (final) has been done. 
            if (data.length === 1 && data[0].winner) {
                resultsContainer.innerHTML = `<p id="theWinner">Winner: ${data[0].winner}</p>`;
            }

            else {
            resultsContainer.innerHTML = data.map(result => `
                <div>
                    <p>${result.first} vs ${result.second || 'No opponent'} - Winner: ${result.winner || 'N/A'}</p>
                </div>
            `).join('');
            }
        })
        .catch(error => console.error('Error fetching results:', error));
}

function nextRound() {
    fetch(`${apiBase}/nextround`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => response.json())
        .then(data => {
            displayPairs(data);
        })
        .catch(error => {
            console.error('Error for next round:', error.message);
        });
}

function reset() {
    fetch(`${apiBase}/reset`, {
        method: 'DELETE', 
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            if (!response.ok) {
             return response.json().then(error => { throw new Error(error.message); });
            }
            return response.json();
        })
        .then(data => {
            console.log('Delete successful:', data);
            document.getElementById('participants').innerHTML = '';
            fetchParticipants();

        })
        .catch(error => {
            console.error('Error:', error);
        });
    window.location.reload(); //... improve later //

}