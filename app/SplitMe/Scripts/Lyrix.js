function deleteGenre(code) {

    $.getJSON('/Administration/Genre/Delete?code=' + code,
		function (data) {
		    		    if (!data.deleted) {
		    		        alert("WARNING - You cannot delete a genre that is currently in use.");
		    		        return false;
		    		    }
		    		    else {
		    		        window.location = '/Administration/Genre/List';
		    		        return false;
		    		    }
		});

}

function deleteArtist(code) {

    $.getJSON('/Administration/Artist/Delete?id=' + code,
		function (data) {
		    if (!data.deleted) {
		        alert("WARNING - You cannot delete a artist that is currently in use.");
		        return false;
		    }
		    else {
		        window.location = '/Administration/Artist/List';
		        return false;
		    }
		});

}

function deleteLyrics(code) {
    
    $.getJSON('/Administration/Question/Delete?code=' + code,
		function (data) {
		    		    if (!data.deleted) {
		    		        alert("WARNING - You cannot delete a lyrics that is currently in use.");
		    		        return false;
		    		    }
		    		    else {
		    		        window.location = '/Administration/Question/List';
		    		        return false;
		    		    }
		});
}