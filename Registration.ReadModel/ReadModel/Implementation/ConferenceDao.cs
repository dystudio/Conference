using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Conference.Common;
using ECommon.Components;
using ECommon.Dapper;

namespace Registration.ReadModel.Implementation
{
    [Component]
    public class ConferenceDao : IConferenceDao
    {
        public ConferenceDetails GetConferenceDetails(string conferenceCode)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<ConferenceDetails>(new { Code = conferenceCode }, "ConferencesView").SingleOrDefault();
            }
        }

        public ConferenceAlias GetConferenceAlias(string conferenceCode)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<ConferenceAlias>(new { Code = conferenceCode }, "ConferencesView").SingleOrDefault();
            }
        }

        public IList<ConferenceAlias> GetPublishedConferences()
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<ConferenceAlias>(new { IsPublished = 1 }, "ConferencesView").ToList();
            }
        }

        public IList<SeatType> GetPublishedSeatTypes(Guid conferenceId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<SeatType>(new { ConferenceId = conferenceId }, "ConferenceSeatTypesView").ToList();
            }
        }

        public IList<SeatTypeName> GetSeatTypeNames(IEnumerable<Guid> seatTypes)
        {
            var distinctIds = seatTypes.Distinct().ToArray();
            if (distinctIds.Length == 0)
            {
                return new List<SeatTypeName>();
            }

            using (var connection = GetConnection())
            {
                var result = new List<SeatTypeName>();
                foreach (var seatId in distinctIds)
                {
                    var seat = connection.QueryList<SeatType>(new { Id = seatId }, "ConferenceSeatTypesView").SingleOrDefault();
                    if (seat != null)
                    {
                        result.Add(new SeatTypeName { Id = seat.Id, Name = seat.Name });
                    }
                }
                return result;
            }
        }

        private IDbConnection GetConnection()
        {
            return new SqlConnection(ConfigSettings.ConnectionString);
        }
    }
}