using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

// Queue player, read matchmaker data,  
public enum MatchmakerPollingResult
{
    Success,
    TicketCreationError,
    TicketCancellationError,
    TicketRetrievalError,
    MatchAssignmentError
}

public class MatchmakingResult
{
    public string ip;
    public int port;
    public MatchmakerPollingResult result;
    public string resultMessage;
}

public class MatchplayMatchmaker : IDisposable
{
    private string lastUsedTicket;
    private CancellationTokenSource cancelToken;

    private const int TicketCooldown = 1000;

    public bool IsMatchmaking { get; private set; }


    // Queue
    public async Task<MatchmakingResult> Matchmake(UserData data)
    {
        cancelToken = new CancellationTokenSource();

        //  Get Prefs
        string queueName = data.userGamePreferences.ToMultiplayQueue();
        //  Q name
        CreateTicketOptions createTicketOptions = new CreateTicketOptions(queueName);
        Debug.Log(createTicketOptions.QueueName);

        //  Players for Q (Teams not implimented)
        List<Player> players = new List<Player>
        {
            new Player(data.userAuthId, data.userGamePreferences)
        };

        try
        {
            // Go to UGS & create player / Q ticket
            IsMatchmaking = true;
            CreateTicketResponse createResult = await MatchmakerService.Instance.CreateTicketAsync(players, createTicketOptions);

            lastUsedTicket = createResult.Id;

            // Find tickets & connect (Ping avail. matches)
            try
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    TicketStatusResponse checkTicket = await MatchmakerService.Instance.GetTicketAsync(lastUsedTicket);

                    if (checkTicket.Type == typeof(MultiplayAssignment))
                    {
                        MultiplayAssignment matchAssignment = (MultiplayAssignment)checkTicket.Value;

                        //  OK!
                        if (matchAssignment.Status == MultiplayAssignment.StatusOptions.Found)
                        {
                            return ReturnMatchResult(MatchmakerPollingResult.Success, "", matchAssignment);
                        }
                        if (matchAssignment.Status == MultiplayAssignment.StatusOptions.Timeout ||
                            matchAssignment.Status == MultiplayAssignment.StatusOptions.Failed)
                        {
                            return ReturnMatchResult(MatchmakerPollingResult.MatchAssignmentError,
                                $"Ticket: {lastUsedTicket} - {matchAssignment.Status} - {matchAssignment.Message}", null);
                        }
                        Debug.Log($"Polled Ticket: {lastUsedTicket} Status: {matchAssignment.Status} ");
                    }

                    await Task.Delay(TicketCooldown);
                }
            }
            catch (MatchmakerServiceException e)
            {
                return ReturnMatchResult(MatchmakerPollingResult.TicketRetrievalError, e.ToString(), null);
            }
        }
        catch (MatchmakerServiceException e)
        {
            return ReturnMatchResult(MatchmakerPollingResult.TicketCreationError, e.ToString(), null);
        }

        return ReturnMatchResult(MatchmakerPollingResult.TicketRetrievalError, "Cancelled Matchmaking", null);
    }

    //  CANCEL
    public async Task CancelMatchmaking()
    {
        if (!IsMatchmaking) { return; }

        IsMatchmaking = false;

        if (cancelToken.Token.CanBeCanceled)
        {
            cancelToken.Cancel();
        }

        if (string.IsNullOrEmpty(lastUsedTicket)) { return; }

        Debug.Log($"Cancelling {lastUsedTicket}");

        await MatchmakerService.Instance.DeleteTicketAsync(lastUsedTicket);
    }

    // Data conversion from UGS (return  IP, Port)
    private MatchmakingResult ReturnMatchResult(MatchmakerPollingResult resultErrorType, string message, MultiplayAssignment assignment)
    {
        IsMatchmaking = false;

        if (assignment != null)
        {
            string parsedIp = assignment.Ip;
            int? parsedPort = assignment.Port;
            if (parsedPort == null)
            {
                return new MatchmakingResult
                {
                    result = MatchmakerPollingResult.MatchAssignmentError,
                    resultMessage = $"Port missing? - {assignment.Port}\n-{assignment.Message}"
                };
            }

            return new MatchmakingResult
            {
                result = MatchmakerPollingResult.Success,
                ip = parsedIp,
                port = (int)parsedPort,
                resultMessage = assignment.Message
            };
        }

        return new MatchmakingResult
        {
            result = resultErrorType,
            resultMessage = message
        };
    }

    public void Dispose()
    {
        _ = CancelMatchmaking();

        cancelToken?.Dispose();
    }
}