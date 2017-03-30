/// <reference path="jquery-2.2.4.js" />

function selPerson(el) {
    $.each($('.table-info'), function (ndex, elem) { $(elem).removeClass('table-info'); });
    $(el).addClass('table-info');

    let selPersonId = $(el).prop('id');
    $('#currentPersonId').val(selPersonId);
    $('#currentCallId').val('');

    let personDetail = $('#tableUserDetail');
    
    $.ajax({
        url: '/api/persons/' + selPersonId,
        type: 'GET',
        dataType: 'json',
        success: function (person) {

            let ln = person.LastName === null ? "не указано" : person.LastName;
            let pt = person.Patronymic === null ? "не указано" : person.Patronymic;
            let ger = person.Gender === 1 ? "М" : person.Gender === 2 ? "Ж" : "не указано";

            let bd = "не указано";
            if (person.BirthDate !== null) {
                bd = person.BirthDate.split('T')[0];
            }

            $('#detSurname').text(ln);
            $('#detName').text(person.FirstName);
            $('#detPatronymic').text(pt);
            $('#detGender').text(ger);
            $('#detBirthDate').text(bd);
            $('#detPhone').text(person.PhoneNumber);
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });

    updateCalls(selPersonId)
};

function updateCalls(personId) {
    let personCalls = $('#tableCallsHistory tbody');
    personCalls.empty();

    $.ajax({
        url: '/api/persons/' + personId + '/calls',
        type: 'GET',
        dataType: 'json',
        success: function (calls) {

            $.each(calls, function (index, call) {
                let d = call.CallDate.split('T');
                let co = call.OrderCost === null ? 0 : call.OrderCost;

                let str = "<tr><td>" + d[0].replace(/-/g, '.') +
               "</td><td>" + call.CallReport +
               "</td><td>" + co +
               "</td><td><button type='button' class='btn btn-default btn-sm' data-toggle='modal' data-target='#editCall' id='" + call.Id + "' onclick='editCallClick(this)' >Изменить</button></td><td>" +
               "<button type='button' class='btn btn-danger btn-sm' id='" + call.Id + "' onclick='deleteCallClick(this)' >Удалить</button></td></tr>";

                personCalls.append(str);
            });
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
};

function onFilter() {
    $('#pageContainer').hide();
    $('#loadingMsg').show();
    $('#currentPersonId').val('');
    
    updateRecordsCount();
    $.ajax({
        url: '/api/persons'+getFilters(),
        type: 'GET',
        dataType: 'json',
        success: function (persons) {
            let personsList = $('#tableUsersList tbody');
            personsList.empty();
            $.each(persons, function (index, person) {
                let row = '<tr id="' + person.Id + '" onclick="selPerson(this);"> <td>' + (person.LastName === null ? '' : person.LastName) +
                    '</td><td>' + person.FirstName +
                    '</td><td>' + (person.Patronymic === null ? '' : person.Patronymic) + '</td></tr>';
                personsList.append(row);
            });
            updatePagesLabel();
            $('#loadingMsg').hide();
            $('#pageContainer').show();
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });   
};

function getFilters() {
    let pageNo = parseInt($('#pageNo').val());
    let pageSize = parseInt($('#itemsPerPage').val());
    let newPageSize = parseInt($('#iPerPage').val());
    let recordsCount = parseInt($('#countOfRecords').val());
    let nameFilter = $('#nameFilter').val();
    let oldNameFilter = $('#oldNameFilter').val();
    let gender = parseInt($('#gender').val());
    let oldGender = parseInt($('#oldGender').val());
    let minAge = parseInt($('#minAge').val()) || 0;
    let oldMinAge = parseInt($('#oldMinAge').val()) || 0;
    let maxAge = parseInt($('#maxAge').val()) || 0;
    let oldMaxAge = parseInt($('#oldMaxAge').val()) || 0;
    let minDaysAfterLastCall = parseInt($('#minDaysAfterLastCall').val()) || 0;
    let oldMinDaysAfterLastCall = parseInt($('#oldMinDaysAfterLastCall').val()) || 0;

    if (maxAge < minAge) {
        maxAge = minAge;
        $('#maxAge').val(minAge);
    }
    
    if( nameFilter !== oldNameFilter ||
        gender !== oldGender ||
        minAge !== oldMinAge ||
        maxAge !== oldMaxAge ||
        minDaysAfterLastCall !== minDaysAfterLastCall ||
        pageSize !== newPageSize) {

        $('#oldNameFilter').val(nameFilter);
        $('#oldGender').val(gender);
        $('#oldMinAge').val(minAge);
        $('#oldMaxAge').val(maxAge);
        $('#oldMinDaysAfterLastCall').val(minDaysAfterLastCall);
        $('#pageNo').val(1);
        pageNo = 1;
        $('#itemsPerPage').val(newPageSize);
        pageSize = newPageSize;        
    }

    var parameters = '?';
    parameters += 'PageNo=' + pageNo + '&PageSize=' + pageSize;
       
    if (nameFilter) {
        parameters += '&NameFilter=' + encodeURIComponent(nameFilter);
    }
    if (gender !== 0) {
        parameters += '&Gender=' + gender;
    }
    if (minAge !== 0) {
        parameters += '&MinAge=' + minAge;
    }
    if (maxAge !== 0) {
        parameters += '&MaxAge=' + maxAge;
    }
    if (minDaysAfterLastCall !== 0) {
        parameters += '&MinDaysAfterLastCall=' + minDaysAfterLastCall;
    }
    
    return parameters;
};

function updateRecordsCount() {
    $.ajax({
        url: '/api/persons/count' + getFilters(),
        type: 'GET',
        dataType: 'json',
        success: function (count) {
            $('#countOfRecords').val(count);           
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
};

function getPagesCount() {
    let rpp = parseInt($('#itemsPerPage').val());
    let rc = parseInt($('#countOfRecords').val());
    return rc <= rpp ? 1 : rc % rpp === 0 ? Math.floor(rc / rpp) : Math.floor(rc / rpp) + 1;
}

function updatePagesLabel() {    
    let pn = $('#pageNo').val();
    $('#pageLabel').text('[' + pn + ' стр. из ' + getPagesCount() + ']');
    $('#iPerPage').val($('#itemsPerPage').val());
};

function nextPage() {
    let pageNo = parseInt($('#pageNo').val());
    if (pageNo === getPagesCount()) return;
    $('#pageNo').val(pageNo + 1);         
    onFilter();
};

function prevPage() {
    let pageNo = parseInt($('#pageNo').val());
    if (pageNo === 1) return;
        $('#pageNo').val(pageNo - 1);       
    onFilter();
};

function clearForm() {
    $('#cName').val('');
    $('#cSurname').val('');
    $('#cPatronymic').val('');
    $('#cBirthDate').val('');
    $('#cGender').val(0);
    $('#cPhoneNumber').val('');

    $('#cMessage').hide();
    $('#cValidation').hide();
    $('#currentPersonId').val('');
};

function isValidDate(date) {
    var matches = /^(\d{4})[-\/.](\d{2})[-\/.](\d{2})$/.exec(date);
    if (matches == null) return false;
    var m = matches[2];
    var y = matches[1] - 1;
    var d = matches[3];
    var composedDate = new Date(y, m, d);
    return composedDate.getDate() == d &&
            composedDate.getMonth() == m &&
            composedDate.getFullYear() == y;
}

function formIsValid() {
    let cName = $('#cName').val();
    let cBirthDate = $('#cBirthDate').val();
    let cPhone = $('#cPhoneNumber').val();

    let reqs = cName && (cBirthDate ? isValidDate(cBirthDate) : true) && cPhone;
    let lens = (cName.length >= 3 &&
        cName.length <= 30) &&
        (cPhone.length >= 5 && cPhone.length <= 20) &&
        $('#cSurname').val().length <= 30 && $('#cPatronymic').val().length <= 30;

    return reqs && lens;
};

function save() {
    if (!formIsValid()) {
        $('#cValidation').show();
        return;
    }
    $('#cValidation').hide();

    let firstName = $('#cName').val();
    if (!firstName) firstName = null;
    let lastName = $('#cSurname').val();
    if (!lastName) lastName = null;
    let patronymic = $('#cPatronymic').val();
    if (!patronymic) patronymic = null;
    let birthDate = $('#cBirthDate').val();
    let gender = $('#cGender').val();
    let phoneNumber = $('#cPhoneNumber').val();

    let personId = $('#currentPersonId').val();
    if (personId) {
        $.ajax({
            url: '/api/persons',
            type: "PUT",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                Id : personId,
                FirstName: firstName,
                LastName: lastName,
                Patronymic: patronymic,
                BirthDate: birthDate,
                PhoneNumber: phoneNumber,
                Gender: gender
            }),
            success: function (data) {
                $('#cMessage').show();
                onFilter();
            },
            error: function (x, y, z) {
                alert(x + '\n' + y + '\n' + z);
            }
        });
    }
    else {
        $.ajax({
            url: '/api/persons',
            type: "POST",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                FirstName: firstName,
                LastName: lastName,
                Patronymic: patronymic,
                BirthDate: birthDate,
                PhoneNumber: phoneNumber,
                Gender: gender
            }),
            success: function (data) {
                $('#cMessage').show();
                onFilter();
            },
            error: function (x, y, z) {
                alert(x + '\n' + y + '\n' + z);
            }
        });
    };    
};

function addPersonClick() {    
    clearForm();
}

function editPersonClick() {
    $('#cMessage').hide();
    $('#cValidation').hide();
    let lst = $('#currentPersonId').val();
    if (!lst) {
        alert('Запись не выбрана. При сохранении будет создана новая');
        return;
    }

    $('#cName').val($('#detName').text());
    $('#cSurname').val($('#detSurname').text());
    $('#cPatronymic').val($('#detPatronymic').text());
    $('#cBirthDate').val($('#detBirthDate').text());
    $('#cGender').val($('#detGender').text() === 'не указано' ? 0 : $('#detGender').text() === 'М' ? 1 : 2);
    $('#cPhoneNumber').val($('#detPhone').text());    
};

function deletePersonClick() {
    let lst = $('.table-info');
    if (lst.length === 0) {
        alert('Ничего не выделено');
        return;
    }
    if (!confirm('Удалить данные о клиенте?')) return;
    let selPersonId = $(lst[0]).prop('id');
    
    $.ajax({
        url: '/api/persons/'+selPersonId,
        type: "DELETE",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $('#tableCallsHistory tbody').empty();
            onFilter();
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
};

function callFormIsValid() {
    let cDate = $('#crCallDate').val();
    let cReport = $('#crCallReport').val();
    let cOrderCost = $('#crOrderCost').val();
    let personId = $('#currentPersonId').val();
    
    let reqs = cReport && isValidDate(cDate) && personId;
    let lens = cReport.length <= 500 && cReport.length >= 5;
    
    return reqs && lens;
};

function clearCallForm() {
    $('#crCallDate').val('');
    $('#crCallReport').text('');
    $('#crOrderCost').val('');

    $('#currentCallId').val('');

    $('#crMessage').hide();
    $('#crValidation').hide();
};

function addCallClick() {
    if (!$('#currentPersonId').val()) {
        alert('Запись не выбрана. Сохранение не возможно');
    }
    clearCallForm();
};

function deleteCallClick(btn) {
    if (!confirm('Удалить отчет о звонке?')) {
        return;
    }

    let personId = $('#currentPersonId').val();
    let callId = $(btn).attr('id');
    $('#currentCallId').val('');

    $.ajax({
        url: '/api/persons/' + personId + '/calls/'+callId,
        type: "DELETE",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            updateCalls(personId);
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
};

function editCallClick(btn) {    
    let personId = $('#currentPersonId').val();
    let callId = $(btn).attr('id');
    $('#currentCallId').val(callId);

    let rowList = $(btn).parent().parent().children();
    let cDate = $(rowList[0]).text();
    let cCallReport = $(rowList[1]).text();
    let cOrderCost = $(rowList[2]).text();

    $('#crCallDate').val(cDate);
    $('#crCallReport').text(cCallReport);
    $('#crOrderCost').val(cOrderCost);
    $('#crMessage').hide();
    $('#crValidation').hide();
};

function saveCall() {
    if (!callFormIsValid()) {
        $('#crValidation').show();
        return;
    }
    $('#crValidation').hide();

    let cDate = $('#crCallDate').val();
    let cReport = $('#crCallReport').val();
    let cOrderCost = $('#crOrderCost').val();
    cOrderCost = parseFloat(cOrderCost) === NaN ? 0 : parseFloat(cOrderCost);

    let personId = $('#currentPersonId').val();
    let callId = $('#currentCallId').val();

    if (callId) {
        $.ajax({
            url: '/api/persons/'+personId+'/calls',
            type: "PUT",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                Id: callId,
                PersonId: personId,
                CallReport: cReport,
                CallDate: cDate,
                OrderCost: cOrderCost
            }),
            success: function (data) {
                $('#crMessage').show();
                updateCalls(personId);
            },
            error: function (x, y, z) {
                alert(x + '\n' + y + '\n' + z);
            }
        });
    }
    else {        
        $.ajax({
            url: '/api/persons/'+personId+'/calls',
            type: "POST",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                PersonId: personId,
                CallReport: cReport,
                CallDate: cDate,
                OrderCost: cOrderCost
            }),
            success: function (data) {
                $('#crMessage').show();
                updateCalls(personId);
            },
            error: function (x, y, z) {
                alert(x + '\n' + y + '\n' + z);
            }
        });
    };
};

$(document).ready(function () {
    onFilter();
});