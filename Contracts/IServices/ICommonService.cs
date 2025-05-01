namespace Contracts.IServices
{
    public interface ICommonService
    {
        /// <summary>
        /// Allows access only if the user id from claims matches with the user id requested
        /// </summary>
        /// <param name="userId"></param>
        /// <exception cref="NotFoundException"></exception>
        public void UserIdAccessCheck(Guid userId);

        /// <summary>
        /// Allows access only if the user is an admin
        /// </summary>
        /// <exception cref="ForbiddenException"></exception>
        public void AdminAccessCheck();

        /// <summary>
        /// Gets user id from claims
        /// </summary>
        /// <returns></returns>
        public Guid GetUserIdFromClaims();

        /// <summary>
        /// Allows access only if the user id from claims matches with the user id from token
        /// </summary>
        /// <param name="userId"></param>
        /// <exception cref="NotFoundException"></exception>
        public void UserIdAccessCheckForAddressOrPayment(Guid userId);
    }
}
