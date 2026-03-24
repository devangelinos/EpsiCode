# Project Structure
**EpsiCodeAPI**: The backend engine. Uses the Repository and Service patterns.

**EpsiCodeWeb**: The MVC Frontend. Communicates with the API via HttpClient and AJAX/Fetch.

**EpsiCode.Tests**: xUnit project containing unit tests for business logic.


# Configure Startup Projects:

1. Right-click the Solution > Configure Startup Projects.

2. Select Multiple startup projects.

3. Set both EpsiCodeAPI and EpsiCodeWeb to Start.

4. Verify API Port:

The default configuration expects the API at https://localhost:7261 (or update the _apiBaseUrl in the Web project's controllers if your port differs).


# Assumptions & tradeoffs 
1. For each new book i set a random default price and number of copies
2. The address that the user enters doesnt get validated (could be picked by google maps but for simplicity its a text box now)

Sidenote:
Added the synch job & SignalR bonus tasks

# API Endpoints
**Books**
* GET /api/books — View inventory.

* POST /api/books/fetch — Manual sync with Potter API.

* PATCH /api/books/{id}/price — Update book pricing.

**Orders**
* POST /api/orders — Create a new order.

* POST /api/orders/{id}/books — Add book (Validates stock).

* DELETE /api/orders/{id}/books/{bookId} — Remove book (Restores stock).
